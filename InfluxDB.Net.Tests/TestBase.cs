using System.Threading.Tasks;
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
        public async Task Setup()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            FixtureRepository = new Fixture();
            VerifyAll = true;

            await FinalizeSetUp();
        }

        [TearDown]
        public async Task TearDown()
        {
            if (VerifyAll)
            {
                _mockRepository.VerifyAll();
            }
            else
            {
                _mockRepository.Verify();
            }

            await FinalizeTearDown();
        }

        [TestFixtureSetUp]
        public async Task TestFixtureSetUp()
        {
            await FinalizeTestFixtureSetUp();
        }

        [TestFixtureTearDown]
        public async Task TestFixtureTearDown()
        {
            await FinalizeTestFixtureTearDown();
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

        protected virtual async Task FinalizeTearDown()
        {
        }

        protected virtual async Task FinalizeTestFixtureTearDown()
        {
        }

        protected virtual async Task FinalizeSetUp()
        {
        }

        protected virtual async Task FinalizeTestFixtureSetUp()
        {
        }
    }
}