﻿// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Diagnostics.Tracing;

namespace DotNetCore.CAP.Diagnostics
{
    [EventSource(Name = "DotNetCore.CAP.EventCounter")]
    public class CapEventCounterSource : EventSource
    {
        public static readonly CapEventCounterSource Log = new CapEventCounterSource();

        private IncrementingEventCounter? _publishPerSecondCounter;
        private IncrementingEventCounter? _consumePerSecondCounter;
        private IncrementingEventCounter? _subscriberInvokePerSecondCounter;

        private EventCounter? _invokeCounter;

        private CapEventCounterSource() { }

        protected override void OnEventCommand(EventCommandEventArgs args)
        {
            if (args.Command == EventCommand.Enable)
            {
                _publishPerSecondCounter ??= new IncrementingEventCounter("published-per-second", this)
                {
                    DisplayName = "Publish Rate",
                    DisplayRateTimeScale = TimeSpan.FromSeconds(1)
                };

                _consumePerSecondCounter ??= new IncrementingEventCounter("consume-per-second", this)
                {
                    DisplayName = "Consume Rate",
                    DisplayRateTimeScale = TimeSpan.FromSeconds(1)
                };

                _subscriberInvokePerSecondCounter ??= new IncrementingEventCounter("invoke-subscriber-per-second", this)
                {
                    DisplayName = "Invoke Subscriber Rate",
                    DisplayRateTimeScale = TimeSpan.FromSeconds(1)
                };

                _invokeCounter ??= new EventCounter("invoke-time", this)
                {
                    DisplayName = "Invoke Subscriber Time",
                    DisplayUnits = "ms"
                };
            }
        }

        public void WritePublishMetrics()
        {
            _publishPerSecondCounter?.Increment();
        }

        public void WriteConsumeMetrics()
        {
            _consumePerSecondCounter?.Increment();
        }

        public void WriteInvokeMetrics()
        {
            _subscriberInvokePerSecondCounter?.Increment();
        }

        public void WriteInvokeTimeMetrics(double elapsedMS)
        {
            _invokeCounter?.WriteMetric(elapsedMS);
        }

        protected override void Dispose(bool disposing)
        {
            _publishPerSecondCounter?.Dispose();
            _consumePerSecondCounter?.Dispose();
            _subscriberInvokePerSecondCounter?.Dispose();
            _invokeCounter?.Dispose();

            _publishPerSecondCounter = null;
            _consumePerSecondCounter = null;
            _subscriberInvokePerSecondCounter = null;
            _invokeCounter = null;

            base.Dispose(disposing);
        }
    }
}
