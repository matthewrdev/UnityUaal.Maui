using System.Runtime.InteropServices;
using Foundation;

namespace iOSBridge
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MachHeader
    {
        public uint magic;     /* mach magic number identifier */
        public int cputype; /* cpu specifier ; cpu_type_t*/
        public int cpusubtype;   /* machine specifier ; cpu_subtype_t */
        public uint filetype;  /* type of file */
        public uint ncmds;     /* number of load commands */
        public uint sizeofcmds;    /* the size of all the load commands */
        public uint flags;     /* flags */
        public uint reserved;  /* reserved */
    }

}