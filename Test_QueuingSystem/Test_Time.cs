using QueuingSystemLibraries.Other;
using Xunit;

namespace Test_QueuingSystem
{
    public class Test_Time
    {
        [InlineData(-1, "01:29")]
        [InlineData(1, "01:31")]
        [InlineData(int.MaxValue, "03:37")]
        [InlineData(int.MinValue, "23:22")]
        [InlineData(60, "02:30")]
        [InlineData(100, "03:10")]
        [InlineData(-60, "00:30")]
        [InlineData(-89, "00:01")]
        [InlineData(-91, "23:59")]
        [Theory]
        public void Add_some_minutes(int minutes, string expected)
        {
            //arrange
            Time sut = new Time(1, 30);
            
            //act
            sut.AddMinutes(minutes);
            
            //assert
            Assert.Equal(expected, sut.ToString());
        }
        
        [InlineData(-1, "00:30")]
        [InlineData(1, "02:30")]
        [InlineData(int.MaxValue, "01:30")]
        [InlineData(int.MinValue, "01:30")]
        [InlineData(60, "13:30")]
        [InlineData(100, "05:30")]
        [InlineData(-60, "13:30")]
        [InlineData(-89, "08:30")]
        [Theory]
        public void Add_some_hours(int hours, string expected)
        {
            //arrange
            Time sut = new Time(1, 30);
            
            //act
            sut.AddHours(hours);
            
            //assert
            Assert.Equal(expected, sut.ToString());
        }
        
        [InlineData(3, 43, "03:43")]
        [InlineData(-3, 43, "00:43")]
        [InlineData(-3, -43, "00:00")]
        [Theory]
        public void Set_time(int hours, int minutes, string expected)
        {
            //arrange
            Time sut = new Time(1, 30);
            
            //act
            sut.SetTime(hours, minutes);
            
            //assert
            Assert.Equal(expected, sut.ToString());
        }

        
    }
}