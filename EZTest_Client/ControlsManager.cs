using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EZTest_Client
{
    class ControlsManager
    {
        private List<Control> textBoxesList;

        public ControlsManager()
        {
            textBoxesList = new List<Control>();
        }

        public Control getControl(int id)
        {
            return textBoxesList[id];
        }

        public void addControl(Control ctrl)
        {
            textBoxesList.Add(ctrl);
        }

        public void removeControl(Control ctrl)
        {
            textBoxesList.Remove(ctrl);
        }

        public void removeAll()
        {
            foreach (var item in textBoxesList)
            {
                textBoxesList.Remove(item);
            }
        }
    }
}
