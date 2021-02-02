using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestMapGenerator
    {
        
        // A Test behaves as an ordinary method
        [Test]
        public void isWall()
        {
            MapGenerator mp = new MapGenerator();
            mp.initialHeight = 20;
            mp.initialWidth = 10;
            
            mp.FillRandomMap();
            Assert.IsTrue(mp.isBorder(0, 1));
            Assert.IsTrue(mp.isBorder(1, 0));
            Assert.IsTrue(mp.isBorder(9, 0));
            Assert.IsTrue(mp.isBorder(2, 19));
            
            Assert.IsFalse(mp.isBorder(8, 8));
            Assert.IsFalse(mp.isBorder(8, 18));
            Assert.IsFalse(mp.isBorder(10, 20));
        }
        [Test]
        public void NewTestScriptSimplePasses()
        {
            MapGenerator mp = new MapGenerator();
            mp.initialHeight = 20;
            mp.initialWidth = 10;
            
            mp.FillRandomMap();
            Assert.IsTrue(mp.isBorder(0, 1));
            Assert.IsTrue(mp.isBorder(1, 0));
            Assert.IsTrue(mp.isBorder(9, 0));
            Assert.IsTrue(mp.isBorder(2, 19));
            
            Assert.IsFalse(mp.isBorder(8, 8));
            Assert.IsFalse(mp.isBorder(8, 18));
            Assert.IsFalse(mp.isBorder(10, 20));
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
