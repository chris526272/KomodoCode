using System;
using TotalPhase;
namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        int i;
        int count;
        //int nelem = 16;
        ushort[] ports = new ushort[16];
        uint[] unique_ids = new uint[16];

        public Form1()
        {
            InitializeComponent();

            // Find all the attached devices
            count = KomodoApi.km_find_devices_ext(16, ports, 16, unique_ids);
            Console.Write("{0:d} ports(s) found:\n", count);

            if (count > 16) count = 16;
            for (i = 0; i < count; ++i)
            {
                // Determine if the device is in-use
                String status = "(avail) ";
                if ((ports[i] & KomodoApi.KM_PORT_NOT_FREE) != 0)
                {
                    ports[i] &= unchecked((ushort)~KomodoApi.KM_PORT_NOT_FREE);
                    status = "(in-use)";
                }

                // Display device port number, in-use status, and serial number
                // Note that each Komodo device has 2 ports
                Console.Write("    port={0,-3:d} {1:s} ({2:d4}-{3:d6})\n",
                              ports[i], status,
                              unique_ids[i] / 1000000,
                              unique_ids[i] % 1000000);
            }
        }
    }
}