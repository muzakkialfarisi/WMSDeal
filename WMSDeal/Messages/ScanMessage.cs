using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSDeal.Messages
{
    public class ScanMessage:ValueChangedMessage<string>
    {
        public ScanMessage(string value):base(value)
        {

        }
    }
}
