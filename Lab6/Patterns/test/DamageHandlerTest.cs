using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using static System.Net.Mime.MediaTypeNames;

/*namespace Patterns.test
{
    [TestClass]
    public class DamageHandlerTest
    {


        [TestMethod]
        public void AbstractDamageHandlerTest()
        {
            Console.WriteLine("Тестирует базовую структуру абстрактного обработчика");
            AbstractDamageHandler testAbstractDamageHandler = new AbstractDamageHandler();
            int testDamage = 0;

            // тестирует завершение обработки если текущий обработчик - последний
            int actualDamage = testAbstractDamageHandler.handle(testDamage);

            Assert.AreEqual(testDamage, actualDamage);

            // тестирует передачу обработки следующему обработчику
            AbstractDamageHandler testNextAbstractDamageHandlerMock = new Mock<AbstractDamageHandler>().Object;
            int testHandledDamage = 10;
            Mock<AbstractDamageHandler>.When(testNextAbstractDamageHandlerMock.handle(testDamage) == 0).thenReturn(testHandledDamage);

            testAbstractDamageHandler.setNext(testNextAbstractDamageHandlerMock);
            actualDamage = testAbstractDamageHandler.handle(testDamage);

            Assertions.assertEquals(testHandledDamage, actualDamage);
            verify(testNextAbstractDamageHandlerMock, times(1)).handle(testDamage);
        }
    }
}*/
