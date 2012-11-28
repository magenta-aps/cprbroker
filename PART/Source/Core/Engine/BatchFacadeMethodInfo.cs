using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine
{
    public class BatchFacadeMethodInfo<TOutput, TSingleItem> : FacadeMethodInfo<TOutput, TSingleItem[]> where TOutput : class, IBasicOutput<TSingleItem[]>, new()
    {
        public BatchFacadeMethodInfo(string appToken, string userToken)
            : base(appToken, userToken)
        {
            this.AggregationFailOption = AggregationFailOption.FailOnAll;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override bool IsValidResult(TSingleItem[] output)
        {
            return base.IsValidResult(output);
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

        public TSingleInputItem[] _CandidateInput;
        public TSingleOutputItem[] _CandidateOutput;


        public BatchSubMethodInfo(TSingleInputItem[] inp)
        {
            States = inp.Select(s => new Status() { Input = s, Output = default(TSingleOutputItem) }).ToArray();

            this.LocalDataProviderOption = LocalDataProviderUsageOption.UseFirst;
            this.FailOnDefaultOutput = true;
            this.FailIfNoDataProvider = true;
        }

        public TOutput Run<TOutput>(IEnumerable<TInterface> providers) where TOutput : IBasicOutput<TSingleOutputItem[]>, new()
        {
            foreach (var prov in providers)
            {
                var candidateStates = States
                        .Where(s => !IsSucceededStatus(s)).ToArray();

                if (candidateStates.Length == 0)
                {
                    break;
                }

                this._CandidateInput = candidateStates
                    .Select(kvp => kvp.Input).ToArray();

                try
                {
                    _CandidateOutput = RunMainMethod(prov);

                    for (int i = 0; i < candidateStates.Length; i++)
                    {
                        candidateStates[i].Output = _CandidateOutput[i];
                    }

                    InvokeUpdateMethod(null);
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
    }
}
