using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MediaPlayer.Keys
{
    public class GlobalHotkey
    {
        /// <summary>
        /// The modifier key require to be pressed for the hot key to be (Ex, Ctrl, Shift, ...)
        /// </summary>
        public ModifierKeys modifierKeys { get; set; }

        /// <summary>
        /// The key required to be pressed for the hotkey to be fired
        /// </summary>
        public Key Key { get; set; }

        /// <summary>
        /// The method to be called when the hot key is pressed
        /// </summary>
        public Action Callback { get; set; }

        /// <summary>
        /// States if the method can be executed
        /// </summary>
        public bool canExecute { get; set; }

        /// <summary>
        /// Initiates a new hotkey with the given modifier, key, callback method, 
        /// and also a boolean stating if the callback can be run (can be changed, see <see cref="CanExecute"/>)
        /// </summary>
        /// <param name="modifierKeys">The modifier key required to be pressed</param>
        /// <param name="key">The hot key required to be pressed for firing hot key</param>
        /// <param name="callback">The method that gets called when the hot key is fired</param>
        /// <param name="canExecute"></param>
        public GlobalHotkey(ModifierKeys modifierKeys, Key key, Action callback, bool canExecute = true)
        {
            this.modifierKeys = modifierKeys;
            Key = key;
            Callback = callback;
            this.canExecute = canExecute;
        }
    }
}
