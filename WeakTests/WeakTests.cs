using System;
using System.Linq;
using Xunit;

public class WeakTests
{
    class Worker
    {
        public static Worker Instance;
        public static int NumVal;

        public void Inc()
            => NumVal++;

        public void Inc(int v)
            => NumVal += v;


        public int Dec()
            => NumVal--;

        public int Dec(int v)
            => NumVal -= v;

        ~Worker()
        {
            Instance = this;
        }

        public void EventHandler(object sender, EventArgs e)
            => NumVal = -NumVal;
    }

    event EventHandler TestEvent;
    event EventHandler<EventArgs> TestEvent1;

    [Fact]
    public void ForEverything()
    {
        var worker = new Worker();
        var worker0 = new Worker();

        {
            Worker.NumVal = 233;
            Assert.Equal(233, Worker.NumVal);

            worker.Inc();
            Assert.Equal(234, Worker.NumVal);

            worker.Inc(20);
            Assert.Equal(254, Worker.NumVal);

            Assert.Equal(234, worker.Dec(20));
            Assert.Equal(234, worker.Dec());
        }

        var weak = worker.ToWeakDelegateTarget();
        var weak0 = worker0.ToWeakDelegateTarget();
        var weak1 = worker.ToWeakDelegateTarget(true);

        TestEvent += weak.GetHandler(worker.EventHandler);
        TestEvent1 += weak1.GetHandler<EventArgs>(worker.EventHandler);

        {
            var inc0 = weak.GetAction(t => t?.Inc());
            var inc1 = weak.GetAction<int>((t, v) => t?.Inc(v));
            var dec0 = weak.GetFunc(t => t?.Dec());
            var dec1 = weak.GetFunc<int, int?>((t, v) => t?.Dec(v));

            Assert.Equal(233, Worker.NumVal);

            inc0();
            Assert.Equal(234, Worker.NumVal);

            inc1(20);
            Assert.Equal(254, Worker.NumVal);

            Assert.Equal(234, dec1(20));
            Assert.Equal(234, dec0());


            TestEvent1(null, null);
            Assert.Equal(-233, Worker.NumVal);

            TestEvent(null, null);
            Assert.Equal(233, Worker.NumVal);

            GC.KeepAlive(worker);
            worker = null;
            GC.Collect();
            //GC.Collect();

            Assert.Equal(233, Worker.NumVal);

            inc0();
            Assert.Equal(233, Worker.NumVal);

            inc1(20);
            Assert.Equal(233, Worker.NumVal);

            Assert.Equal(null, dec0());
            Assert.Equal(null, dec1(20));
        }

        {
            var inc0 = weak0.GetAction(t => t?.Inc());
            var inc1 = weak0.GetAction<int>((t, v) => t?.Inc(v));
            var dec0 = weak0.GetFunc(t => t?.Dec());
            var dec1 = weak0.GetFunc<int, int?>((t, v) => t?.Dec(v));

            Assert.Equal(233, Worker.NumVal);

            inc0();
            Assert.Equal(234, Worker.NumVal);

            inc1(20);
            Assert.Equal(254, Worker.NumVal);

            Assert.Equal(234, dec1(20));
            Assert.Equal(234, dec0());

            ((Reference<Worker>)weak0).Target = Worker.Instance;
            GC.KeepAlive(worker0);
            worker0 = null;
            GC.Collect();
            //GC.Collect();

            Assert.Equal(233, Worker.NumVal);

            inc0();
            Assert.Equal(234, Worker.NumVal);

            inc1(20);
            Assert.Equal(254, Worker.NumVal);

            Assert.Equal(234, dec1(20));
            Assert.Equal(234, dec0());
        }

        {
            var inc0 = weak1.GetAction(t => t?.Inc());
            var inc1 = weak1.GetAction<int>((t, v) => t?.Inc(v));
            var dec0 = weak1.GetFunc(t => t?.Dec());
            var dec1 = weak1.GetFunc<int, int?>((t, v) => t?.Dec(v));

            Assert.Equal(233, Worker.NumVal);

            inc0();
            Assert.Equal(234, Worker.NumVal);

            inc1(20);
            Assert.Equal(254, Worker.NumVal);

            Assert.Equal(234, dec1(20));
            Assert.Equal(234, dec0());

            TestEvent(null, null);
            Assert.Equal(233, Worker.NumVal);

            TestEvent1(null, null);
            Assert.Equal(-233, Worker.NumVal);

            Worker.Instance = null;
            GC.Collect();
            //GC.Collect();

            TestEvent1(null, null);
            Assert.Equal(-233, Worker.NumVal);

            Assert.Equal(-233, Worker.NumVal);

            inc0();
            Assert.Equal(-233, Worker.NumVal);

            inc1(20);
            Assert.Equal(-233, Worker.NumVal);

            Assert.Equal(null, dec0());
            Assert.Equal(null, dec1(20));
        }
    }
}
