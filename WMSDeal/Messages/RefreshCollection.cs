using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSDeal.Messages
{
    public class RefreshCollection:ValueChangedMessage<string>
    {
        public RefreshCollection(string value):base(value)
        {

        }
    }
}
