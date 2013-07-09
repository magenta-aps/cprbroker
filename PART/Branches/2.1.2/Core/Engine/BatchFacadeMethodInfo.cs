using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine
{
    public class BatchFacadeMethodInfo<TInterface, TOutput, TSingleInputItem, TSingleOutputItem> : FacadeMethodInfo<TOutput, TSingleOutputItem[]>
        where TInterface : class, IDataProvider
        where TOutput : class, IBasicOutput<TSingleOutputItem[]>, new()
    {
        public TSingleInputItem[] input;

        public BatchFacadeMethodInfo(string appToken, string userToken)
            : base(appToken, userToken)
        {
            this.AggregationFailOption = AggregationFailOption.FailOnAll;
        }

        public override sealed void Initialize()
        {
            this.MainSubMethod = CreateMainSubMethod();
            this.SubMethodInfos = new SubMethodInfo[] { MainSubMethod };
        }

        public BatchSubMethodInfo<TInterface, TSingleInputItem, TSingleOutputItem> MainSubMethod { get; private set; }

        protected virtual BatchSubMethodInfo<TInterface, TSingleInputItem, TSingleOutputItem> CreateMainSubMethod()
        {
            return null;
        }

        public override bool IsValidResult(TSingleOutputItem[] output)
        {
            return base.IsValidResult(output);
        }

        public TOutput Run(SubMethodRunState[] subMethodRunStates)
        {
            var mainsubMethodRunState = subMethodRunStates[0];
            return MainSubMethod.Run<TOutput>(mainsubMethodRunState.DataProviders.Select(p => p as TInterface));
        }
    }

    public class BatchSubMethodInfo<TInterface, TSingleInputItem, TSingleOutputItem> : SubMethodInfo<TInterface, TSingleOutputItem[]>
        where TInterface : class, IDataProvider
    {
        public class Status
        {
            public TSingleInputItem Input;
            public TSingleOutputItem Output;
            public string PossibleErrorReason = Schemas.Part.StandardReturType.DataProviderFailedText;
        }

        public Status[] States;

        public virtual bool IsSucceededStatus(Status s)
        {
            return !object.Equals(s.Output, default(TSingleOutputItem));
        }


        public BatchSubMethodInfo(TSingleInputItem[] inp)
        {
            States = inp.Select(s => new Status() { Input = s, Output = default(TSingleOutputItem) }).ToArray();

            this.LocalDataProviderOption = SourceUsageOrder.LocalThenExternal;
            this.FailOnDefaultOutput = true;
            this.FailIfNoDataProvider = true;
        }

        public virtual TSingleOutputItem[] Run(TInterface prov, TSingleInputItem[] input)
        {
            return default(TSingleOutputItem[]);
        }

        public TOutput Run<TOutput>(IEnumerable<TInterface> providers) where TOutput : IBasicOutput<TSingleOutputItem[]>, new()
        {
            foreach (var prov in providers)
            {
                var currentStates = States
                        .Where(s => !IsSucceededStatus(s)).ToArray();

                if (currentStates.Length == 0)
                {
                    break;
                }

                var currentInput = currentStates
                    .Select(kvp => kvp.Input).ToArray();

                try
                {
                    var currentOutput = Run(prov, currentInput);
                    var currentSucceededStates = new List<Status>();
                    for (int i = 0; i < currentStates.Length; i++)
                    {
                        currentStates[i].Output = currentOutput[i];
                        if (IsSucceededStatus(currentStates[i]))
                            currentSucceededStates.Add(currentStates[i]);
                    }

                    if (prov is IExternalDataProvider)
                    {
                        InvokeUpdateMethod(
                            currentSucceededStates.Select(s => s.Input).ToArray(),
                            currentSucceededStates.Select(s => s.Output).ToArray()
                            );
                    }
                }
                catch (Exception ex)
                {
                    Local.Admin.LogException(ex);
                }
            }

            // Now create the result
            var failed = States.Where(s => !IsSucceededStatus(s)).ToArray();
            var result = States.Select(s => s.Output).ToArray();
            var succeededCount = States.Length - failed.Length;

            var ret = new TOutput();
            ret.SetMainItem(result);

            if (succeededCount == 0)
            {
                ret.StandardRetur = StandardReturType.Create(HttpErrorCode.DATASOURCE_UNAVAILABLE);
            }
            else if (succeededCount < States.Length)
            {
                var failuresAndReasons = failed
                    .GroupBy(s => s.PossibleErrorReason)
                    .ToDictionary(
                        g => g.Key,
                        g => g.ToArray()
                            .Select(s => string.Format("{0}", s.Input))
                    );
                ret.StandardRetur = StandardReturType.PartialSuccess(failuresAndReasons);
            }
            else
            {
                ret.StandardRetur = StandardReturType.OK();
            }
            return ret;
        }

        public override sealed void InvokeUpdateMethod(object result)
        {
            // TODO: Remove from inheritance
            base.InvokeUpdateMethod(result);
        }

        public virtual void InvokeUpdateMethod(TSingleInputItem[] input, TSingleOutputItem[] output)
        {

        }
    }
}
