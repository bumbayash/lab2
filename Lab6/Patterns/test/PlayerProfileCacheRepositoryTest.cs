using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Patterns;
using static System.Net.Mime.MediaTypeNames;
using Moq;
using System.CodeDom;

namespace Patterns.test
{
    [TestClass]
    public class PlayerProfileCacheRepositoryTest
    {
        [TestMethod]
        public void testReadCache()
        {
            Console.WriteLine("Тестирует кеширующую логику на чтение");
            var dbRepositoryMock = new Mock<PlayerProfileDBRepository>();
            string testPlayerName = "ace";
            PlayerProfile testPlayerProfile = new PlayerProfile(testPlayerName, 0);
            dbRepositoryMock.Setup(ser => ser.getProfile(testPlayerName)).Returns(testPlayerProfile);
            PlayerProfileCacheRepository playerProfileCacheRepository = setupProxyRepository(dbRepositoryMock.Object);

            PlayerProfile actualPlayerProfile;
            for (int i = 0; i< 10; i++) {
                actualPlayerProfile = playerProfileCacheRepository.getProfile(testPlayerName);
                Assert.AreEqual(testPlayerProfile.getName(), actualPlayerProfile.getName());
                Assert.AreEqual(testPlayerProfile.getScore(), actualPlayerProfile.getScore());
            }

            dbRepositoryMock.Verify(ser => ser.getProfile(testPlayerName), Times.AtLeastOnce());
            dbRepositoryMock.Verify(ser => ser.getProfile(testPlayerName), Times.AtMostOnce());
        }


        [TestMethod]
        public void testWriteCache()
        {
            Console.WriteLine("Тестирует write-through кеширующую логику на запись");
            var dbRepositoryMock = new Mock<PlayerProfileDBRepository>();
            string testPlayerName = "ace";
            int testInitialScore = 0;
            int testUpdatedScore = 100;
            PlayerProfile testPlayerProfile = new PlayerProfile(testPlayerName, testInitialScore);
            dbRepositoryMock.Setup(ser => ser.getProfile(testPlayerName)).Returns(testPlayerProfile);
            PlayerProfileCacheRepository playerProfileCacheRepository = setupProxyRepository(dbRepositoryMock.Object);

            // если профиль есть в кеше - запись должна произойти и в кеш, и в базу
            // при этом чтения из базы должно быть только в первый раз
            //noinspection UnusedAssignment
            PlayerProfile actualUpdatedPlayerProfile = playerProfileCacheRepository.getProfile(testPlayerName);
            playerProfileCacheRepository.updateHighScore(testPlayerName, testUpdatedScore);
            actualUpdatedPlayerProfile = playerProfileCacheRepository.getProfile(testPlayerName);

            Assert.AreEqual(testUpdatedScore, actualUpdatedPlayerProfile.getScore());
            dbRepositoryMock.Verify(ser => ser.getProfile(testPlayerName), Times.Exactly(1));
            dbRepositoryMock.Object.updateHighScore(testPlayerName, testUpdatedScore);
            dbRepositoryMock.Verify(ser => ser.getProfile(testPlayerName), Times.Exactly(1));
            dbRepositoryMock.Object.getProfile(testPlayerName);
        }

        // с помощью рефлексии подкидываем мок-прокси в заместителя
        private PlayerProfileCacheRepository setupProxyRepository(PlayerProfileDBRepository mockDependency)
        {
            try
            {
                /*PlayerProfileCacheRepository playerProfileCacheRepository = new PlayerProfileCacheRepository();
                Type repository = playerProfileCacheRepository.GetType();
                repository. (true);
                repository. set(playerProfileCacheRepository, mockDependency);
                return playerProfileCacheRepository;*/
                return new PlayerProfileCacheRepository();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
