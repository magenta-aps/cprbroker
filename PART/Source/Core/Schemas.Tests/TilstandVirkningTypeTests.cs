using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.Schemas
{
    [TestFixture]
    class TilstandVirkningTypeTests
    {
        [Test]
        public void ToVirkningType_Correct_CorrectActorRef(
            [Values("93kljakljslajfdas", "jakjkljf")]string actorRefValue)
        {
            var virkning = new TilstandVirkningType() { AktoerRef = new UnikIdType { Item = actorRefValue } };
            var result = virkning.ToVirkningType();
            Assert.AreEqual(actorRefValue, result.AktoerRef.Item);
        }

        [Test]
        public void ToVirkningType_Correct_CorrectCommentText(
            [Values("93kljakljslajfdas", "jakjkljf")]string commentText)
        {
            var virkning = new TilstandVirkningType() { CommentText = commentText };
            var result = virkning.ToVirkningType();
            Assert.AreEqual(commentText, result.CommentText);
        }

        [Test]
        public void ToVirkningType_Correct_CorrectFraTidspunkt(
            [Values(1, 3, 5)]int yearOffset)
        {
            DateTime date = DateTime.Today.AddYears(yearOffset);
            var virkning = new TilstandVirkningType() { FraTidspunkt = TidspunktType.Create(date) };
            var result = virkning.ToVirkningType();
            Assert.AreEqual(date, result.FraTidspunkt.ToDateTime());
        }

        [Test]
        public void ToVirkningType_Correct_NullTilTidspunkt(
            [Values(1, 3, 5)]int yearOffset)
        {
            DateTime date = DateTime.Today.AddYears(yearOffset);
            var virkning = new TilstandVirkningType() { FraTidspunkt = TidspunktType.Create(date) };
            var result = virkning.ToVirkningType();
            Assert.Null(result.TilTidspunkt);
        }
    }
}
