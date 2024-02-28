using Moq.AutoMock;

namespace Shared.SystemUnderTest {
    public class SpecAutoMocker<TInterface, TClass> where TClass : class, TInterface {
        protected AutoMocker Mocker;
        protected TClass TestableImplementation;

        protected void Init(bool instantiate = true, bool enablePrivate = false) {
            Mocker = new AutoMocker();

            Setup();

            if (instantiate) {
                TestableImplementation = Mocker.CreateInstance<TClass>(enablePrivate);
            }

            Arrange();
            Act();
        }

        protected virtual void Setup() { }
        protected virtual void Arrange() { }
        protected virtual void Act() { }
    }
}
