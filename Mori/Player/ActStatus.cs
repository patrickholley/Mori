using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mori
{
    class ActStatus
    {
        public ActStatus(bool IsActing, bool IsJumping, int XOffset, int YOffset) {
            this.IsActing = IsActing;
            this.IsJumping = IsJumping;
            this.XOffset = XOffset;
            this.YOffset = YOffset;
        }
        
        public bool IsActing { get; set; }
        public bool IsJumping { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }
    }
}
