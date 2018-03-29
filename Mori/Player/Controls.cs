using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mori
{
    class Controls
    {
        public Keys AttackKey { get; set; } = Keys.J;
        public Keys ClimbKey { get; set; } = Keys.K;
        public Keys JumpKey { get; set; } = Keys.W;
        public Keys ThrowKey { get; set; } = Keys.L;
        public Keys SlideKey { get; set; } = Keys.S;
        public Keys LeftKey { get; set; } = Keys.A;
        public Keys RightKey { get; set; } = Keys.D;
    }
}
