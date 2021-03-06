﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyCaffe.param;
using MyCaffe.basecode;
using System.Threading;
using MyCaffe.common;
using System.Drawing;
using System.Diagnostics;
using MyCaffe.db.image;
using MyCaffe.basecode.descriptors;
using MyCaffe.gym;

namespace MyCaffe.test
{
    [TestClass]
    public class TestMyCaffeGym
    {
        [TestMethod]
        public void TestCartPoleWithOutUi()
        {
            MyCaffeGymTest test = new MyCaffeGymTest();

            try
            {
                foreach (IMyCaffeGymTest t in test.Tests)
                {
                    t.TestCartPole(false);
                }
            }
            finally
            {
                test.Dispose();
            }
        }
    }


    interface IMyCaffeGymTest : ITest
    {
        void TestCartPole(bool bShowUi);
    }

    class MyCaffeGymTest : TestBase
    {
        public MyCaffeGymTest(EngineParameter.Engine engine = EngineParameter.Engine.DEFAULT)
            : base("MyCaffe Gym Test", TestBase.DEFAULT_DEVICE_ID, engine)
        {
        }

        protected override ITest create(common.DataType dt, string strName, int nDeviceID, EngineParameter.Engine engine)
        {
            if (dt == common.DataType.DOUBLE)
                return new MyCaffeGymTest<double>(strName, nDeviceID, engine);
            else
                return new MyCaffeGymTest<float>(strName, nDeviceID, engine);
        }
    }

    class MyCaffeGymTest<T> : TestEx<T>, IMyCaffeGymTest
    {
        public MyCaffeGymTest(string strName, int nDeviceID, EngineParameter.Engine engine)
            : base(strName, null, nDeviceID)
        {
        }

        protected override void dispose()
        {
            base.dispose();
        }

        public void TestCartPole(bool bShowUi)
        {
            m_log.WriteHeader("Test Gym - Open");
            GymCollection col = new GymCollection();
            col.Load();

            string strName = "Cart-Pole";
            IXMyCaffeGym igym = col.Find(strName);
            Assert.AreEqual(igym != null, true);

            igym = igym.Clone();
            Assert.AreEqual(igym != null, true);

            igym.Initialize(m_log, null);

            Dictionary<string, int> rgActions = igym.GetActionSpace();
            Assert.AreEqual(rgActions.Count, 2);
            Assert.AreEqual(rgActions.ContainsKey("MoveLeft"), true);
            Assert.AreEqual(rgActions.ContainsKey("MoveRight"), true);
            Assert.AreEqual(rgActions["MoveLeft"], 0);
            Assert.AreEqual(rgActions["MoveRight"], 1);

            igym.Reset();
            igym.Step(0);
            igym.Step(1);

            Thread.Sleep(5000);

            igym.Close();
        }
    }
}
