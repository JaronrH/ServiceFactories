namespace ServiceFactories.Tests.Components
{
    public class TestImplementation : ITestService
    {
        public TestImplementation(string name)
        {
            Name = name;
        }

        #region Implementation of ITest

        public string Name { get; }

        #endregion
    }
}
