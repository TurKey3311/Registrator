using Xunit;
using Registrator.services.utils;

namespace Registrator
{
    public class MaskPhoneFormatterTests
    {
        private readonly Mask _mask = new Mask();

        [Fact]
        public void MaskPhoneNumber_ValidNumber_ReturnsFormattedPhone()
        {
            string input = "+79123456789";
            var result = _mask.MaskPhoneNumber(input);
            Assert.True(result.IsSuccess);
            Assert.Equal("+7(912)345-67-89", result.Value);
        }

        [Fact]
        public void MaskPhoneNumber_EmptyString_ReturnsFailure()
        {
            string input = "";
            var result = _mask.MaskPhoneNumber(input);
            Assert.False(result.IsSuccess);
            Assert.Equal("Номер телефона не может быть пустым", result.ErrorMessage);
        }
    }
}