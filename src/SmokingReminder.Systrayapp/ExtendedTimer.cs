// Copyright (c) 2024 Denis Mikhailov. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Contact and Information: https://github.com/denzor200/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SmokingReminder.Systrayapp
{
    internal class ExtendedTimer : IDisposable
    {
        private Timer timer;
        private DateTime m_lastTime;
        private bool disposed = false;

        public ExtendedTimer(double val)
        {
            timer = new Timer(val);
            timer.Elapsed += (s, e) =>
            {
                this.m_lastTime = DateTime.Now;
                if (this.Enabled && this.Elapsed != null)
                    this.Elapsed(s, e);
            };
            timer.Enabled = true;
            timer.AutoReset = true;

            this.m_lastTime = DateTime.Now;
            this.Enabled = false;
        }
        public double TimeLeft
        {
            get
            {
                var dueTime = this.m_lastTime.AddMilliseconds(this.Interval);
                var timeLeft = (dueTime - DateTime.Now).TotalMilliseconds;
                return timeLeft >= 0 ? timeLeft : 0;
            }
        }
        public void Stop()
        {
            this.Enabled = false;
        }
        public double Interval { get { return timer.Interval; } }
        public ElapsedEventHandler Elapsed {  get; set; }
        public bool Enabled { get; set; }
        public bool AutoReset { get { return true; } }


        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(disposing: true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    timer.Dispose();
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        // Use C# finalizer syntax for finalization code.
        // This finalizer will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide finalizer in types derived from this class.
        ~ExtendedTimer()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(disposing: false) is optimal in terms of
            // readability and maintainability.
            Dispose(disposing: false);
        }
    }
}
