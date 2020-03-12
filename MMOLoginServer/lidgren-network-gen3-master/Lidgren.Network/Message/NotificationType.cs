using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lidgren.Network.Message
{
    public enum NotificationType
    {
        InvalidCharacterName,
        InvalidUsernameOrPassword,
        EmailOrUsernameExists,
        ///Need more of this later!!
        ///Need to specify notification messages without sending hardcoded string!
        ///Send notification MessageType
        ///and then NotificationType
        ///Becouse of language difference!
    }
}
