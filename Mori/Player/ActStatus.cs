using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mori
{
    class ActStatus
    {
        public ActStatus(bool IsActing, bool IsClimbing, bool IsJumping,
            bool IsRunning, int XOffset, int YOffset) {
            this.IsActing = IsActing;
            this.IsClimbing = IsClimbing;
            this.IsJumping = IsJumping;
            this.IsRunning = IsRunning;
            this.XOffset = XOffset;
            this.YOffset = YOffset;
        }
        
        public bool IsActing { get; set; }
        public bool IsClimbing { get; set; }
        public bool IsJumping { get; set; }
        public bool IsRunning { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }
    }
}
