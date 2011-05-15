using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PsGet.Communications {
    public class EventQueue : ProducerSubscriberQueue<Action> {
        public void Run() {
            try {
                Action act;
                while ((act = WaitForItem()) != null) {
                    act();
                }
            } catch(OperationCanceledException) {
                return;
            }
        }
    }
}
