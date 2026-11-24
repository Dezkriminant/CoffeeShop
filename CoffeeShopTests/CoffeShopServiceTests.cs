using Xunit;
using CoffeeShop;
using System.Collections.Generic;

namespace CoffeeShopTests
{
    public class CoffeeShopServiceTests
    {
        private readonly CoffeeShopService _service;
        
        public CoffeeShopServiceTests()
        {
            _service = new CoffeeShopService();
        }

        
        // ТЕСТЫ ДЛЯ МЕТОДА IsValidDrink 
        

        [Theory]
        [InlineData(1)] 
        [InlineData(4)] 
        [InlineData(5)] 
        public void IsValidDrink_ExistingDrink_ReturnsTrue(int drinkId)
        {
            bool result = _service.IsValidDrink(drinkId);
            
            Assert.True(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(99)]
        [InlineData(-1)]
        public void IsValidDrink_NonExistingDrink_ReturnsFalse(int drinkId)
        {
            
            bool result = _service.IsValidDrink(drinkId);

            
            Assert.False(result);
        }

        
        // ТЕСТЫ ДЛЯ МЕТОДА CanAddIngredient 
        

        [Fact]
        public void CanAddIngredient_ValidAndNewIngredient_ReturnsTrue()
        {
            
            var addedIngredients = new HashSet<int>(); 
            
            bool result = _service.CanAddIngredient(1, addedIngredients); 
            
            Assert.True(result);
        }

        [Fact]
        public void CanAddIngredient_AlreadyAddedIngredient_ReturnsFalse()
        {
            var addedIngredients = new HashSet<int> { 1 }; 
            
            bool result = _service.CanAddIngredient(1, addedIngredients);
            
            Assert.False(result);
        }

        [Fact]
        public void CanAddIngredient_NonExistingIngredient_ReturnsFalse()
        {
            var addedIngredients = new HashSet<int>();
            
            bool result = _service.CanAddIngredient(99, addedIngredients);
            
            Assert.False(result);
        }

        
        // ТЕСТЫ ДЛЯ МЕТОДА CalculateTotal 
        

        [Fact]
        public void CalculateTotal_OnlyDrink_ReturnsDrinkPrice()
        {
            int drinkId = 4;
            var addedIngredients = new HashSet<int>();
            
            int total = _service.CalculateTotal(drinkId, addedIngredients);
            
            Assert.Equal(85, total);
        }

        [Fact]
        public void CalculateTotal_DrinkWithIngredients_ReturnsCorrectSum()
        {
            int drinkId = 4; 
            var addedIngredients = new HashSet<int> { 1, 3 }; 
            
            int total = _service.CalculateTotal(drinkId, addedIngredients);
            
            Assert.Equal(110, total); 
        }

        [Fact]
        public void CalculateTotal_FreeIngredient_DoesNotChangePrice()
        {
            int drinkId = 5; 
            var addedIngredients = new HashSet<int> { 2 }; 
            
            int total = _service.CalculateTotal(drinkId, addedIngredients);
            
            Assert.Equal(40, total);
        }

        


// ТЕСТЫ ДЛЯ МЕТОДА ProcessPayment 
       

        [Fact]
        public void ProcessPayment_NotEnoughFunds_ReturnsErrorAndNoRevenue()
        {
            int orderTotal = 110;
            int cash = 100;
            
            var result = _service.ProcessPayment(orderTotal, cash);
            
            Assert.False(result.IsSuccess);
            Assert.Equal("Недостаточно средств", result.Message);
            Assert.Equal(0, result.Change);
            Assert.Equal(0, _service.TotalRevenue); 
        }

        [Fact]
        public void ProcessPayment_ExactAmount_ReturnsSuccessAndZeroChange()
        {
            int orderTotal = 110;
            int cash = 110;
            
            var result = _service.ProcessPayment(orderTotal, cash);
            
            Assert.True(result.IsSuccess);
            Assert.Equal("Спасибо за покупку!", result.Message);
            Assert.Equal(0, result.Change);
            Assert.Equal(110, _service.TotalRevenue); 
        }

        [Fact]
        public void ProcessPayment_MoreThanEnough_ReturnsSuccessAndCorrectChange()
        {
            int orderTotal = 110;
            int cash = 500;
            
            var result = _service.ProcessPayment(orderTotal, cash);
            
            Assert.True(result.IsSuccess);
            Assert.Equal(390, result.Change); 
            Assert.Equal(110, _service.TotalRevenue);
        }

        [Fact]
        public void ProcessPayment_MultipleSuccessfulPayments_AccumulatesRevenue()
        {
            _service.ProcessPayment(100, 100); 
            _service.ProcessPayment(150, 200); 
            
            Assert.Equal(250, _service.TotalRevenue); 
        }
    }
}