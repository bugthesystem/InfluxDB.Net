using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace InfluxDB.Net.Tests
{
    // NOTE: http://stackoverflow.com/questions/106907/making-code-internal-but-available-for-unit-testing-from-other-projects

    [TestFixture]
    public class TestBase
    {
        [SetUp]
        public void Setup()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            FixtureRepository = new Fixture();
            VerifyAll = true;
            FinalizeSetUp();
        }

        [TearDown]
        public void TearDown()
        {
            if (VerifyAll)
            {
                _mockRepository.VerifyAll();
            }
            else
            {
                _mockRepository.Verify();
            }

            FinalizeTearDown();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            FinalizeTestFixtureTearDown();
        }

        private MockRepository _mockRepository;
        protected IFixture FixtureRepository { get; set; }
        protected bool VerifyAll { get; set; }

        protected Mock<T> MockFor<T>() where T : class
        {
            return _mockRepository.Create<T>();
        }

        protected Mock<T> MockFor<T>(params object[] @params) where T : class
        {
            return _mockRepository.Create<T>(@params);
        }

        protected void EnableCustomization(ICustomization customization)
        {
            customization.Customize(FixtureRepository);
        }

        protected virtual void FinalizeTearDown()
        {
        }

        protected virtual void FinalizeTestFixtureTearDown()
        {
        }

        protected virtual void FinalizeSetUp()
        {
        }
    }
}