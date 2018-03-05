using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphMed_Beta.FileHandling;

namespace GraphMed_UnitTest
{
    [TestClass]
    public class FileHandlerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            FileHandler.ValidateCSV("sdadsdsads"); 
        }
    }
}
