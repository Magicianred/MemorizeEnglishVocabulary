using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WpfApp2
{
    [TestClass]
    public class UnitTest1
    {
        #region Public Methods
        [TestMethod]
        public void TestMethod1()
        {
            var script = LongManScriptHelper.GetScript("Society");

            var words = new[] {"Table", "Card"};

             EnToTrCache.StartToInitializeCache(words);
        }
        #endregion
    }
}