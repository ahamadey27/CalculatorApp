using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using CalculatorWeb.Services;

namespace CalculatorWeb.Tests.Services
{
    public class CalculatorServiceTest
    {
        private readonly ICalculatorService calculatorService; //instance of method
        public CalculatorServiceTest()
        {
            calculatorService = new CalculatorService(); // Instantiating the concrete service for unit tests
        }



        [Fact]
        public void Subtract_TwoNumber_ReturnCorrectDifference()
        {
            //Arrange
            decimal num1 = 2.0m;
            decimal num2 = 10.0m;
            decimal expected = -8.0m;

            //Act
            decimal actual = calculatorService.Subtract(num1, num2);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Multiply_MultiplyNumbers_ReturnsResults()
        {
            //Arrange
            decimal num1 = 10.0m;
            decimal num2 = 5.0m;
            decimal expected = 50.0m;

            //Act
            decimal actual = calculatorService.Multiply(num1, num2);

            // Assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void divide_ValidNumbers_ReturnsQuotient()
        {
            //Arrange
            decimal num1 = 8.0m;
            decimal num2 = 4.0m;
            decimal expected = 2.0m;

            //Act
            decimal actual = calculatorService.Divide(num1, num2);

            // Assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void Dvide_ByZero_ThrowsDivdeByZeroExceptionMethod()
        {
            //Arrange
            decimal num1 = 10m;
            decimal num2 = 0m;

            //Act & Assert
            Assert.Throws<DivideByZeroException>(() => calculatorService.Divide(num1, num2));
        }
    }
}