using Bongo.Models.ModelValidations;
using NUnit.Framework;

namespace Bongo.Models
{
    [TestFixture]
    public class DateInFutureAttributeTests
    {
        [TestCase(100, ExpectedResult = true)]
        [TestCase(0, ExpectedResult = false)]
        public bool DateValidator_InputExpectedDateRange_DateValidity(int addTime)
        {
            DateInFutureAttribute dateInFutureAttribute = new(() => DateTime.Now);
            return dateInFutureAttribute.IsValid(DateTime.Now.AddSeconds(addTime));
        }

        [Test]
        public void DateValidator_NotValidDate_ReturnErrorMessage()
        {
            var result = new DateInFutureAttribute();
            Assert.AreEqual("Date must be in the future", result.ErrorMessage);
        }
    }
}
