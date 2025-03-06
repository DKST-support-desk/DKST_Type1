using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PLOSDeviceConnectionManager
{
    public partial class UcBitView : UserControl
    {
        public UcBitView()
        {
            InitializeComponent();

            lblP0B0.BackColor = Color.Lime;
            lblP0B1.BackColor = Color.Lime;
            lblP0B2.BackColor = Color.Lime;
            lblP0B3.BackColor = Color.Lime;
            lblP0B4.BackColor = Color.Lime;
            lblP0B5.BackColor = Color.Lime;
            lblP0B6.BackColor = Color.Lime;
            lblP0B7.BackColor = Color.Lime;

            lblP1B0.BackColor = Color.Lime;
            lblP1B1.BackColor = Color.Lime;
            lblP1B2.BackColor = Color.Lime;
            lblP1B3.BackColor = Color.Lime;
            lblP1B4.BackColor = Color.Lime;
            lblP1B5.BackColor = Color.Lime;
            lblP1B6.BackColor = Color.Lime;
            lblP1B7.BackColor = Color.Lime;
        }

        public void ChangeBitStatus(int port, int bit, bool status)
        {
            switch (port)
            {
                case 0:
                    switch (bit)
                    {
                        case 0:
                            lblP0B0.BackColor = status ? Color.Red : Color.Lime;
                            break;
                        case 1:
                            lblP0B1.BackColor = status ? Color.Red : Color.Lime;
                            break;
                        case 2:
                            lblP0B2.BackColor = status ? Color.Red : Color.Lime;
                            break;
                        case 3:
                            lblP0B3.BackColor = status ? Color.Red : Color.Lime;
                            break;
                        case 4:
                            lblP0B4.BackColor = status ? Color.Red : Color.Lime;
                            break;
                        case 5:
                            lblP0B5.BackColor = status ? Color.Red : Color.Lime;
                            break;
                        case 6:
                            lblP0B6.BackColor = status ? Color.Red : Color.Lime;
                            break;
                        case 7:
                            lblP0B7.BackColor = status ? Color.Red : Color.Lime;
                            break;
                        default:
                            break;
                    }
                    break;
                case 1:
                    switch (bit)
                    {
                        case 0:
                            lblP1B0.BackColor = status ? Color.Red : Color.Lime;
                            break;
                        case 1:
                            lblP1B1.BackColor = status ? Color.Red : Color.Lime;
                            break;
                        case 2:
                            lblP1B2.BackColor = status ? Color.Red : Color.Lime;
                            break;
                        case 3:
                            lblP1B3.BackColor = status ? Color.Red : Color.Lime;
                            break;
                        case 4:
                            lblP1B4.BackColor = status ? Color.Red : Color.Lime;
                            break;
                        case 5:
                            lblP1B5.BackColor = status ? Color.Red : Color.Lime;
                            break;
                        case 6:
                            lblP1B6.BackColor = status ? Color.Red : Color.Lime;
                            break;
                        case 7:
                            lblP1B7.BackColor = status ? Color.Red : Color.Lime;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;

            }
        }

        public void ChangeBitStatus(int bit, bool status)
        {
            switch (bit)
            {
                case 0:
                    lblP0B0.BackColor = status ? Color.Red : Color.Lime;
                    break;
                case 1:
                    lblP0B1.BackColor = status ? Color.Red : Color.Lime;
                    break;
                case 2:
                    lblP0B2.BackColor = status ? Color.Red : Color.Lime;
                    break;
                case 3:
                    lblP0B3.BackColor = status ? Color.Red : Color.Lime;
                    break;
                case 4:
                    lblP0B4.BackColor = status ? Color.Red : Color.Lime;
                    break;
                case 5:
                    lblP0B5.BackColor = status ? Color.Red : Color.Lime;
                    break;
                case 6:
                    lblP0B6.BackColor = status ? Color.Red : Color.Lime;
                    break;
                case 7:
                    lblP0B7.BackColor = status ? Color.Red : Color.Lime;
                    break;
                case 8:
                    lblP1B0.BackColor = status ? Color.Red : Color.Lime;
                    break;
                case 9:
                    lblP1B1.BackColor = status ? Color.Red : Color.Lime;
                    break;
                case 10:
                    lblP1B2.BackColor = status ? Color.Red : Color.Lime;
                    break;
                case 11:
                    lblP1B3.BackColor = status ? Color.Red : Color.Lime;
                    break;
                case 12:
                    lblP1B4.BackColor = status ? Color.Red : Color.Lime;
                    break;
                case 13:
                    lblP1B5.BackColor = status ? Color.Red : Color.Lime;
                    break;
                case 14:
                    lblP1B6.BackColor = status ? Color.Red : Color.Lime;
                    break;
                case 15:
                    lblP1B7.BackColor = status ? Color.Red : Color.Lime;
                    break;
                default:
                    break;
            }
        }
    }
}
