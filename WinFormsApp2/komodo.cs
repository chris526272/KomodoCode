/*=========================================================================
| Komodo Interface Library
|--------------------------------------------------------------------------
| Copyright (c) 2011-2022 Total Phase, Inc.
| All rights reserved.
| www.totalphase.com
|
| Redistribution and use in source and binary forms, with or without
| modification, are permitted provided that the following conditions
| are met:
|
| - Redistributions of source code must retain the above copyright
|   notice, this list of conditions and the following disclaimer.
|
| - Redistributions in binary form must reproduce the above copyright
|   notice, this list of conditions and the following disclaimer in the
|   documentation and/or other materials provided with the distribution.
|
| - Neither the name of Total Phase, Inc. nor the names of its
|   contributors may be used to endorse or promote products derived from
|   this software without specific prior written permission.
|
| THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
| "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
| LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
| FOR A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE
| COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
| INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
| BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
| LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
| CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
| LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
| ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
| POSSIBILITY OF SUCH DAMAGE.
|--------------------------------------------------------------------------
| To access Komodo devices through the API:
|
| 1) Use one of the following shared objects:
|      komodo.so       --  Linux shared object
|      komodo.dll      --  Windows dynamic link library
|
| 2) Along with one of the following language modules:
|      komodo.c/h      --  C/C++ API header file and interface module
|      komodo_py.py    --  Python API
|      komodo.bas      --  Visual Basic 6 API
|      komodo.cs       --  C# .NET source
|      komodo_net.dll  --  Compiled .NET binding
 ========================================================================*/

using System;
using System.Reflection;
using System.Runtime.InteropServices;

//[assembly: AssemblyTitleAttribute("Komodo .NET binding")]
[assembly: AssemblyDescriptionAttribute(".NET binding for Komodo")]
//[assembly: AssemblyCompanyAttribute("Total Phase, Inc.")]
//[assembly: AssemblyProductAttribute("Komodo")]
[assembly: AssemblyCopyrightAttribute("Total Phase, Inc. 2022")]

namespace TotalPhase {

public enum km_status_t : int {
    /* General codes (0 to -99) */
    KM_OK                      =    0,
    KM_UNABLE_TO_LOAD_LIBRARY  =   -1,
    KM_UNABLE_TO_LOAD_DRIVER   =   -2,
    KM_UNABLE_TO_LOAD_FUNCTION =   -3,
    KM_INCOMPATIBLE_LIBRARY    =   -4,
    KM_INCOMPATIBLE_DEVICE     =   -5,
    KM_COMMUNICATION_ERROR     =   -6,
    KM_UNABLE_TO_OPEN          =   -7,
    KM_UNABLE_TO_CLOSE         =   -8,
    KM_INVALID_HANDLE          =   -9,
    KM_CONFIG_ERROR            =  -10,
    KM_PARAM_ERROR             =  -11,
    KM_FUNCTION_NOT_AVAILABLE  =  -12,
    KM_FEATURE_NOT_ACQUIRED    =  -13,
    KM_NOT_DISABLED            =  -14,
    KM_NOT_ENABLED             =  -15,

    /* CAN codes (-100 to -199) */
    KM_CAN_READ_EMPTY          = -101,
    KM_CAN_SEND_TIMEOUT        = -102,
    KM_CAN_SEND_FAIL           = -103,
    KM_CAN_ASYNC_EMPTY         = -104,
    KM_CAN_ASYNC_MAX_REACHED   = -105,
    KM_CAN_ASYNC_PENDING       = -106,
    KM_CAN_ASYNC_TIMEOUT       = -107,
    KM_CAN_AUTO_BITRATE_FAIL   = -108
}

public enum km_can_ch_t : int {
    KM_CAN_CH_A = 0,
    KM_CAN_CH_B = 1
}

public enum km_power_t : int {
    KM_TARGET_POWER_QUERY = 0x02,
    KM_TARGET_POWER_OFF   = 0x00,
    KM_TARGET_POWER_ON    = 0x01
}

public enum km_pin_bias_t : int {
    KM_PIN_BIAS_TRISTATE = 0x00,
    KM_PIN_BIAS_PULLUP   = 0x01,
    KM_PIN_BIAS_PULLDOWN = 0x02
}

public enum km_pin_trigger_t : int {
    KM_PIN_TRIGGER_NONE         = 0x00,
    KM_PIN_TRIGGER_RISING_EDGE  = 0x01,
    KM_PIN_TRIGGER_FALLING_EDGE = 0x02,
    KM_PIN_TRIGGER_BOTH_EDGES   = 0x03
}

public enum km_pin_drive_t : int {
    KM_PIN_DRIVE_NORMAL            = 0x00,
    KM_PIN_DRIVE_INVERTED          = 0x01,
    KM_PIN_DRIVE_OPEN_DRAIN        = 0x02,
    KM_PIN_DRIVE_OPEN_DRAIN_PULLUP = 0x03
}

public enum km_pin_source_t : int {
    KM_PIN_SRC_SOFTWARE_CTL       = 0x00,
    KM_PIN_SRC_ALL_ERR_CAN_A      = 0x11,
    KM_PIN_SRC_BIT_ERR_CAN_A      = 0x12,
    KM_PIN_SRC_FORM_ERR_CAN_A     = 0x13,
    KM_PIN_SRC_STUFF_ERR_CAN_A    = 0x14,
    KM_PIN_SRC_OTHER_ERR_CAN_A    = 0x15,
    KM_PIN_SRC_ALL_ERR_CAN_B      = 0x21,
    KM_PIN_SRC_BIT_ERR_CAN_B      = 0x22,
    KM_PIN_SRC_FORM_ERR_CAN_B     = 0x23,
    KM_PIN_SRC_STUFF_ERR_CAN_B    = 0x24,
    KM_PIN_SRC_OTHER_ERR_CAN_B    = 0x25,
    KM_PIN_SRC_ALL_ERR_CAN_BOTH   = 0x31,
    KM_PIN_SRC_BIT_ERR_CAN_BOTH   = 0x32,
    KM_PIN_SRC_FORM_ERR_CAN_BOTH  = 0x33,
    KM_PIN_SRC_STUFF_ERR_CAN_BOTH = 0x34,
    KM_PIN_SRC_OTHER_ERR_CAN_BOTH = 0x35
}


public class KomodoApi {

/*=========================================================================
| HELPER FUNCTIONS / CLASSES
 ========================================================================*/
static long tp_min(long x, long y) { return x < y ? x : y; }

private class GCContext {
    GCHandle[] handles;
    int index;
    public GCContext () {
        handles = new GCHandle[16];
        index   = 0;
    }
    public void add (GCHandle gch) {
        handles[index] = gch;
        index++;
    }
    public void free () {
        while (index != 0) {
            index--;
            handles[index].Free();
        }
    }
}

/*=========================================================================
| VERSION
 ========================================================================*/
[DllImport ("komodo")]
private static extern int km_c_version ();

public const int KM_API_VERSION    = 0x0146;   // v1.70
public const int KM_REQ_SW_VERSION = 0x010a;   // v1.10

private static short KM_SW_VERSION;
private static short KM_REQ_API_VERSION;
private static bool  KM_LIBRARY_LOADED;

static KomodoApi () {
    KM_SW_VERSION      = (short)(km_c_version() & 0xffff);
    KM_REQ_API_VERSION = (short)((km_c_version() >> 16) & 0xffff);
    KM_LIBRARY_LOADED  = 
        ((KM_SW_VERSION >= KM_REQ_SW_VERSION) &&
         (KM_API_VERSION >= KM_REQ_API_VERSION));
}

/*=========================================================================
| STATUS CODES
 ========================================================================*/
/*
 * All API functions return an integer which is the result of the
 * transaction, or a status code if negative.  The status codes are
 * defined as follows:
 */
// enum km_status_t  (from declaration above)
//     KM_OK                      =    0
//     KM_UNABLE_TO_LOAD_LIBRARY  =   -1
//     KM_UNABLE_TO_LOAD_DRIVER   =   -2
//     KM_UNABLE_TO_LOAD_FUNCTION =   -3
//     KM_INCOMPATIBLE_LIBRARY    =   -4
//     KM_INCOMPATIBLE_DEVICE     =   -5
//     KM_COMMUNICATION_ERROR     =   -6
//     KM_UNABLE_TO_OPEN          =   -7
//     KM_UNABLE_TO_CLOSE         =   -8
//     KM_INVALID_HANDLE          =   -9
//     KM_CONFIG_ERROR            =  -10
//     KM_PARAM_ERROR             =  -11
//     KM_FUNCTION_NOT_AVAILABLE  =  -12
//     KM_FEATURE_NOT_ACQUIRED    =  -13
//     KM_NOT_DISABLED            =  -14
//     KM_NOT_ENABLED             =  -15
//     KM_CAN_READ_EMPTY          = -101
//     KM_CAN_SEND_TIMEOUT        = -102
//     KM_CAN_SEND_FAIL           = -103
//     KM_CAN_ASYNC_EMPTY         = -104
//     KM_CAN_ASYNC_MAX_REACHED   = -105
//     KM_CAN_ASYNC_PENDING       = -106
//     KM_CAN_ASYNC_TIMEOUT       = -107
//     KM_CAN_AUTO_BITRATE_FAIL   = -108


/*=========================================================================
| GENERAL TYPE DEFINITIONS
 ========================================================================*/
/* Komodo handle type definition */
/* typedef Komodo => int */

/*
 * Komodo version matrix.
 *
 * This matrix describes the various version dependencies
 * of Komodo components.  It can be used to determine
 * which component caused an incompatibility error.
 *
 * All version numbers are of the format:
 *   (major << 8) | minor
 *
 * ex. v1.20 would be encoded as:  0x0114
 */
[StructLayout(LayoutKind.Sequential)]
public struct KomodoVersion {
    /* Software, firmware, and hardware versions. */
    public ushort software;
    public ushort firmware;
    public ushort hardware;

    /*
     * Firmware revisions that are compatible with this software version.
     * The top 16 bits gives the maximum accepted fw revision.
     * The lower 16 bits gives the minimum accepted fw revision.
     */
    public uint   fw_revs_for_sw;

    /*
     * Hardware revisions that are compatible with this software version.
     * The top 16 bits gives the maximum accepted hw revision.
     * The lower 16 bits gives the minimum accepted hw revision.
     */
    public uint   hw_revs_for_sw;

    /* Software requires that the API interface must be >= this version. */
    public ushort api_req_by_sw;
}

/*
 * Komodo feature set
 *
 * This bitmask field describes the features available on this device.
 *
 * When returned by km_features() or km_open_ext(), it refers to the
 * potential features of the device.
 * When used as a parameter by km_enable() or km_disable(), it refers
 * to the features that the user wants to use.
 * And when returned by km_disable(), it refers to the features currently
 * in use by the user.
 */
public const uint KM_FEATURE_GPIO_LISTEN = 0x00000001;
public const uint KM_FEATURE_GPIO_CONTROL = 0x00000002;
public const uint KM_FEATURE_GPIO_CONFIG = 0x00000004;
public const uint KM_FEATURE_CAN_A_LISTEN = 0x00000008;
public const uint KM_FEATURE_CAN_A_CONTROL = 0x00000010;
public const uint KM_FEATURE_CAN_A_CONFIG = 0x00000020;
public const uint KM_FEATURE_CAN_B_LISTEN = 0x00000040;
public const uint KM_FEATURE_CAN_B_CONTROL = 0x00000080;
public const uint KM_FEATURE_CAN_B_CONFIG = 0x00000100;

/*=========================================================================
| GENERAL API
 ========================================================================*/
/*
 * Get a list of ports to which Komodo devices are attached.
 *
 * nelem   = maximum number of elements to return
 * devices = array into which the port numbers are returned
 *
 * Each element of the array is written with the port number.
 * Devices that are in-use are ORed with KM_PORT_NOT_FREE (0x8000).
 *
 * ex.  devices are attached to ports 0, 1, 2
 *      ports 0 and 2 are available, and port 1 is in-use.
 *      array => 0x0000, 0x8001, 0x0002
 *
 * If the array is NULL, it is not filled with any values.
 * If there are more devices than the array size, only the
 * first nmemb port numbers will be written into the array.
 *
 * Returns the number of devices found, regardless of the
 * array size.
 */
public const ushort KM_PORT_NOT_FREE = 0x8000;
public const ushort KM_PORT_NUM_MASK = 0x00ff;
public static int km_find_devices (
    int       num_ports,
    ushort[]  ports
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    int ports_num_ports = (int)tp_min(num_ports, ports.Length);
    return net_km_find_devices(ports_num_ports, ports);
}

/*
 * Get a list of ports to which Komodo devices are attached.
 *
 * This function is the same as km_find_devices() except that
 * it returns the unique IDs of each Komodo device.  The IDs
 * are guaranteed to be non-zero if valid.
 *
 * The IDs are the unsigned integer representation of the 10-digit
 * serial numbers.
 */
public static int km_find_devices_ext (
    int       num_ports,
    ushort[]  ports,
    int       num_ids,
    uint[]    unique_ids
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    int ports_num_ports = (int)tp_min(num_ports, ports.Length);
    int unique_ids_num_ids = (int)tp_min(num_ids, unique_ids.Length);
    return net_km_find_devices_ext(ports_num_ports, ports, unique_ids_num_ids, unique_ids);
}

/*
 * Open the Komodo port.
 *
 * The port number is a zero-indexed integer.
 *
 * The port number is the same as that obtained from the
 * km_find_devices() function above.
 *
 * Returns an Komodo handle, which is guaranteed to be
 * greater than zero if it is valid.
 *
 * This function is recommended for use in simple applications
 * where extended information is not required.  For more complex
 * applications, the use of km_open_ext() is recommended.
 */
public static int km_open (
    int  port_number
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_open(port_number);
}

/*
 * Open the Komodo port, returning extended information
 * in the supplied structure.  Behavior is otherwise identical
 * to km_open() above.  If 0 is passed as the pointer to the
 * structure, this function is exactly equivalent to km_open().
 *
 * The structure is zeroed before the open is attempted.
 * It is filled with whatever information is available.
 *
 * For example, if the firmware version is not filled, then
 * the device could not be queried for its version number.
 *
 * The feature list is a bitmap of Komodo resources, with the same
 * mapping as obtained from the km_features() function below.
 * Details on the bitmask are found above.
 *
 * This function is recommended for use in complex applications
 * where extended information is required.  For more simple
 * applications, the use of km_open() is recommended.
 */
[StructLayout(LayoutKind.Sequential)]
public struct KomodoExt {
    /* Version matrix */
    public KomodoVersion version;

    /* Features of this device. */
    public uint          features;
}

public static int km_open_ext (
    int            port_number,
    ref KomodoExt  km_ext
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_open_ext(port_number, ref km_ext);
}

/* Close the Komodo port. */
public static int km_close (
    int  komodo
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_close(komodo);
}

/*
 * Return the port for this Komodo handle.
 *
 * The port number is a zero-indexed integer identical to those
 * returned by km_find_devices() above.  This includes the count of
 * interfaces in use in the upper byte.
 */
public static int km_port (
    int  komodo
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_port(komodo);
}

/*
 * Return the device features as a bit-mask of values, or an error code
 * if the handle is not valid.  Details on the bitmask are found above.
 */
public static int km_features (
    int  komodo
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_features(komodo);
}

/*
 * Return the unique ID for this Komodo adapter.
 * IDs are guaranteed to be non-zero if valid.
 * The ID is the unsigned integer representation of the
 * 10-digit serial number.
 */
public static uint km_unique_id (
    int  komodo
)
{
    if (!KM_LIBRARY_LOADED) return 0;
    return net_km_unique_id(komodo);
}

/*
 * Return the status string for the given status code.
 * If the code is not valid or the library function cannot
 * be loaded, return a NULL string.
 */
public static string km_status_string (
    int  status
)
{
    if (!KM_LIBRARY_LOADED) return null;
    return Marshal.PtrToStringAnsi(net_km_status_string(status));
}

/*
 * Return the version matrix for the device attached to the
 * given handle.  If the handle is 0 or invalid, only the
 * software and required api versions are set.
 */
public static int km_version (
    int                komodo,
    ref KomodoVersion  version
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_version(komodo, ref version);
}

/*
 * Sleep for the specified number of milliseconds.
 * Accuracy depends on the operating system scheduler.
 * Returns the number of milliseconds slept.
 */
public static uint km_sleep_ms (
    uint  milliseconds
)
{
    if (!KM_LIBRARY_LOADED) return 0;
    return net_km_sleep_ms(milliseconds);
}

/*
 * Acquire device features.
 * Returns the features that are currently acquired.
 */
public static int km_acquire (
    int   komodo,
    uint  features
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_acquire(komodo, features);
}

/*
 * Release device features.
 * Returns the features that are still acquired.
 */
public static int km_release (
    int   komodo,
    uint  features
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_release(komodo, features);
}


/*=========================================================================
| CAN API
 ==========================================================================
| These special timeout constants can be used with the functions
| km_timeout and km_can_async_collec*/
public const int KM_TIMEOUT_IMMEDIATE = 0;
public const int KM_TIMEOUT_INFINITE = -1;
/*
 * Set the timeout of the km_can_read function to the specified
 * number of milliseconds.
 */
public static int km_timeout (
    int   komodo,
    uint  timeout_ms
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_timeout(komodo, timeout_ms);
}

/* Standard enumeration for the CAN channels available on the Komodo. */
// enum km_can_ch_t  (from declaration above)
//     KM_CAN_CH_A = 0
//     KM_CAN_CH_B = 1

/* CAN Bus state constants. */
public const int KM_CAN_BUS_STATE_LISTEN_ONLY = 0x00000001;
public const int KM_CAN_BUS_STATE_CONTROL = 0x00000002;
public const int KM_CAN_BUS_STATE_WARNING = 0x00000004;
public const int KM_CAN_BUS_STATE_ACTIVE = 0x00000008;
public const int KM_CAN_BUS_STATE_PASSIVE = 0x00000010;
public const int KM_CAN_BUS_STATE_OFF = 0x00000020;
/* Retreive the current bus state of the supplied CAN channel */
public static int km_can_query_bus_state (
    int          komodo,
    km_can_ch_t  channel,
    ref byte     bus_state,
    ref byte     rx_error,
    ref byte     tx_error
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_can_query_bus_state(komodo, channel, ref bus_state, ref rx_error, ref tx_error);
}

/*
 * Set the capture latency to the specified number of milliseconds.
 * This number determines the minimum time that a read call will
 * block if there is no available data.  Lower times result in
 * faster turnaround at the expense of reduced buffering.  Setting
 * this parameter too low can cause packets to be dropped.
 */
public static int km_latency (
    int   komodo,
    uint  latency_ms
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_latency(komodo, latency_ms);
}

/* Config mask for km_can_configure */
public const uint KM_CAN_CONFIG_NONE = 0x00000000;
public const uint KM_CAN_CONFIG_LISTEN_SELF = 0x00000001;
public static int km_can_configure (
    int   komodo,
    uint  config
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_can_configure(komodo, config);
}

/*
 * Set the bus timeout.  If a zero is passed as the timeout,
 * the timeout is unchanged and the current timeout is returned.
 */
public static int km_can_bus_timeout (
    int          komodo,
    km_can_ch_t  channel,
    ushort       timeout_ms
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_can_bus_timeout(komodo, channel, timeout_ms);
}

/*
 * Set the CAN bit rate in hertz on the given channel.  If a zero is
 * passed as the bitrate, the bitrate is unchanged.  In all cases, the
 * call will return the bitrate that will be in effect.
 */
public const int KM_KHZ = 1000;
public const int KM_MHZ = 1000000;
public static int km_can_bitrate (
    int          komodo,
    km_can_ch_t  channel,
    uint         bitrate_hz
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_can_bitrate(komodo, channel, bitrate_hz);
}

public static int km_can_auto_bitrate (
    int          komodo,
    km_can_ch_t  channel
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_can_auto_bitrate(komodo, channel);
}

public static int km_can_auto_bitrate_ext (
    int          komodo,
    km_can_ch_t  channel,
    uint         num_bitrates_hz,
    uint[]       bitrates_hz
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    uint bitrates_hz_num_bitrates_hz = (uint)tp_min(num_bitrates_hz, bitrates_hz.Length);
    return net_km_can_auto_bitrate_ext(komodo, channel, bitrates_hz_num_bitrates_hz, bitrates_hz);
}

/* Get the sample rate in hertz. */
public static int km_get_samplerate (
    int  komodo
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_get_samplerate(komodo);
}

/* Configure the target power.  Returns power status or error code. */
// enum km_power_t  (from declaration above)
//     KM_TARGET_POWER_QUERY = 0x02
//     KM_TARGET_POWER_OFF   = 0x00
//     KM_TARGET_POWER_ON    = 0x01

/* Set the target power for the specified CAN channel. */
public static int km_can_target_power (
    int          komodo,
    km_can_ch_t  channel,
    km_power_t   power
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_can_target_power(komodo, channel, power);
}

/* Enable the Komodo. */
public static int km_enable (
    int  komodo
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_enable(komodo);
}

/* Disable the Komodo. */
public static int km_disable (
    int  komodo
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_disable(komodo);
}

/* Status mask for km_can_read */
public const uint KM_READ_TIMEOUT = 0x80000000;
public const uint KM_READ_ERR_OVERFLOW = 0x40000000;
public const uint KM_READ_END_OF_CAPTURE = 0x20000000;
public const uint KM_READ_CAN_ERR = 0x00000100;
public const uint KM_READ_CAN_ERR_FULL_MASK = 0x000000ff;
public const uint KM_READ_CAN_ERR_POS_MASK = 0x0000001f;
public const uint KM_READ_CAN_ERR_POS_SOF = 0x00000003;
public const uint KM_READ_CAN_ERR_POS_ID28_21 = 0x00000002;
public const uint KM_READ_CAN_ERR_POS_ID20_18 = 0x00000006;
public const uint KM_READ_CAN_ERR_POS_SRTR = 0x00000004;
public const uint KM_READ_CAN_ERR_POS_IDE = 0x00000005;
public const uint KM_READ_CAN_ERR_POS_ID17_13 = 0x00000007;
public const uint KM_READ_CAN_ERR_POS_ID12_5 = 0x0000000f;
public const uint KM_READ_CAN_ERR_POS_ID4_0 = 0x0000000e;
public const uint KM_READ_CAN_ERR_POS_RTR = 0x0000000c;
public const uint KM_READ_CAN_ERR_POS_RSVD_1 = 0x0000000d;
public const uint KM_READ_CAN_ERR_POS_RSVD_0 = 0x00000009;
public const uint KM_READ_CAN_ERR_POS_DLC = 0x0000000b;
public const uint KM_READ_CAN_ERR_POS_DF = 0x0000000a;
public const uint KM_READ_CAN_ERR_POS_CRC_SEQ = 0x00000008;
public const uint KM_READ_CAN_ERR_POS_CRC_DEL = 0x00000018;
public const uint KM_READ_CAN_ERR_POS_ACK_SLOT = 0x00000019;
public const uint KM_READ_CAN_ERR_POS_ACK_DEL = 0x0000001b;
public const uint KM_READ_CAN_ERR_POS_EOF = 0x0000001a;
public const uint KM_READ_CAN_ERR_POS_INTRMSN = 0x00000012;
public const uint KM_READ_CAN_ERR_POS_AEF = 0x00000011;
public const uint KM_READ_CAN_ERR_POS_PEF = 0x00000016;
public const uint KM_READ_CAN_ERR_POS_TDB = 0x00000013;
public const uint KM_READ_CAN_ERR_POS_ERR_DEL = 0x00000017;
public const uint KM_READ_CAN_ERR_POS_OVRFLG = 0x0000001c;
public const uint KM_READ_CAN_ERR_DIR_MASK = 0x00000020;
public const uint KM_READ_CAN_ERR_DIR_TX = 0x00000000;
public const uint KM_READ_CAN_ERR_DIR_RX = 0x00000020;
public const uint KM_READ_CAN_ERR_TYPE_MASK = 0x000000c0;
public const uint KM_READ_CAN_ERR_TYPE_BIT = 0x00000000;
public const uint KM_READ_CAN_ERR_TYPE_FORM = 0x00000040;
public const uint KM_READ_CAN_ERR_TYPE_STUFF = 0x00000080;
public const uint KM_READ_CAN_ERR_TYPE_OTHER = 0x000000c0;
public const uint KM_READ_CAN_ARB_LOST = 0x00000200;
public const uint KM_READ_CAN_ARB_LOST_POS_MASK = 0x000000ff;
/* GPIO Configuration */
public const byte KM_GPIO_PIN_1_CONFIG = 0x00;
public const byte KM_GPIO_PIN_2_CONFIG = 0x01;
public const byte KM_GPIO_PIN_3_CONFIG = 0x02;
public const byte KM_GPIO_PIN_4_CONFIG = 0x03;
public const byte KM_GPIO_PIN_5_CONFIG = 0x04;
public const byte KM_GPIO_PIN_6_CONFIG = 0x05;
public const byte KM_GPIO_PIN_7_CONFIG = 0x06;
public const byte KM_GPIO_PIN_8_CONFIG = 0x07;
/* GPIO Mask */
public const byte KM_GPIO_PIN_1_MASK = 0x01;
public const byte KM_GPIO_PIN_2_MASK = 0x02;
public const byte KM_GPIO_PIN_3_MASK = 0x04;
public const byte KM_GPIO_PIN_4_MASK = 0x08;
public const byte KM_GPIO_PIN_5_MASK = 0x10;
public const byte KM_GPIO_PIN_6_MASK = 0x20;
public const byte KM_GPIO_PIN_7_MASK = 0x40;
public const byte KM_GPIO_PIN_8_MASK = 0x80;
/* Event mask for km_can_read */
public const uint KM_EVENT_DIGITAL_INPUT = 0x00000100;
public const uint KM_EVENT_DIGITAL_INPUT_MASK = 0x000000ff;
public const uint KM_EVENT_DIGITAL_INPUT_1 = 0x00000001;
public const uint KM_EVENT_DIGITAL_INPUT_2 = 0x00000002;
public const uint KM_EVENT_DIGITAL_INPUT_3 = 0x00000004;
public const uint KM_EVENT_DIGITAL_INPUT_4 = 0x00000008;
public const uint KM_EVENT_DIGITAL_INPUT_5 = 0x00000010;
public const uint KM_EVENT_DIGITAL_INPUT_6 = 0x00000020;
public const uint KM_EVENT_DIGITAL_INPUT_7 = 0x00000040;
public const uint KM_EVENT_DIGITAL_INPUT_8 = 0x00000080;
public const uint KM_EVENT_CAN_BUS_STATE_LISTEN_ONLY = 0x00001000;
public const uint KM_EVENT_CAN_BUS_STATE_CONTROL = 0x00002000;
public const uint KM_EVENT_CAN_BUS_STATE_WARNING = 0x00004000;
public const uint KM_EVENT_CAN_BUS_STATE_ACTIVE = 0x00008000;
public const uint KM_EVENT_CAN_BUS_STATE_PASSIVE = 0x00010000;
public const uint KM_EVENT_CAN_BUS_STATE_OFF = 0x00020000;
public const uint KM_EVENT_CAN_BUS_BITRATE = 0x00040000;
[StructLayout(LayoutKind.Sequential)]
public struct km_can_info_t {
    public ulong       timestamp;
    public uint        status;
    public uint        events;
    public km_can_ch_t channel;
    public uint        bitrate_hz;
    public byte        host_gen;
    public byte        rx_error_count;
    public byte        tx_error_count;
    public uint        overflow_count;
}

[StructLayout(LayoutKind.Sequential)]
public struct km_can_packet_t {
    public byte remote_req;
    public byte extend_addr;
    public byte dlc;
    public uint id;
}

/*
 * Read a single CAN packet from the Komodo data stream.
 * This will block for timeout_ms milliseconds; 0 will return
 * immediately, and MAXINT will block indefinitely.
 * timestamp is in units of nanoseconds.
 */
public static int km_can_read (
    int                  komodo,
    ref km_can_info_t    info,
    ref km_can_packet_t  pkt,
    int                  num_bytes,
    byte[]               data
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    int data_num_bytes = (int)tp_min(num_bytes, data.Length);
    return net_km_can_read(komodo, ref info, ref pkt, data_num_bytes, data);
}

/* Flags mask */
public const byte KM_CAN_ONE_SHOT = 0x01;
/* Submit a CAN packet to the Komodo data stream, asynchronously. */
public static int km_can_async_submit (
    int              komodo,
    km_can_ch_t      channel,
    byte             flags,
    km_can_packet_t  pkt,
    int              num_bytes,
    byte[]           data
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    int data_num_bytes = (int)tp_min(num_bytes, data.Length);
    return net_km_can_async_submit(komodo, channel, flags, ref pkt, data_num_bytes, data);
}

/*
 * Collect a response to a CAN packet submitted to the Komodo data
 * stream, asynchronously.
 */
public static int km_can_async_collect (
    int       komodo,
    uint      timeout_ms,
    ref uint  arbitration_count
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_can_async_collect(komodo, timeout_ms, ref arbitration_count);
}

/*
 * Write a stream of bytes to the CAN slave device.  The return
 * value of the function is a status code.
 */
public static int km_can_write (
    int              komodo,
    km_can_ch_t      channel,
    byte             flags,
    km_can_packet_t  pkt,
    int              num_bytes,
    byte[]           data,
    ref uint         arbitration_count
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    int data_num_bytes = (int)tp_min(num_bytes, data.Length);
    return net_km_can_write(komodo, channel, flags, ref pkt, data_num_bytes, data, ref arbitration_count);
}


/*=========================================================================
| GPIO API
 ==========================================================================
| Enumeration of input GPIO pin bias configuration*/
// enum km_pin_bias_t  (from declaration above)
//     KM_PIN_BIAS_TRISTATE = 0x00
//     KM_PIN_BIAS_PULLUP   = 0x01
//     KM_PIN_BIAS_PULLDOWN = 0x02

/* Enumeration of input GPIO pin trigger edge condition. */
// enum km_pin_trigger_t  (from declaration above)
//     KM_PIN_TRIGGER_NONE         = 0x00
//     KM_PIN_TRIGGER_RISING_EDGE  = 0x01
//     KM_PIN_TRIGGER_FALLING_EDGE = 0x02
//     KM_PIN_TRIGGER_BOTH_EDGES   = 0x03

/*
 * Configure a GPIO pin to act as an input.  The return value
 * of the function is a status code
 */
public static int km_gpio_config_in (
    int   komodo,
    byte  pin_number,
    byte  bias,
    byte  trigger
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_gpio_config_in(komodo, pin_number, bias, trigger);
}

/* Enumeration of output GPIO pin driver configurations. */
// enum km_pin_drive_t  (from declaration above)
//     KM_PIN_DRIVE_NORMAL            = 0x00
//     KM_PIN_DRIVE_INVERTED          = 0x01
//     KM_PIN_DRIVE_OPEN_DRAIN        = 0x02
//     KM_PIN_DRIVE_OPEN_DRAIN_PULLUP = 0x03

/* Enumeration of output GPIO pin sources. */
// enum km_pin_source_t  (from declaration above)
//     KM_PIN_SRC_SOFTWARE_CTL       = 0x00
//     KM_PIN_SRC_ALL_ERR_CAN_A      = 0x11
//     KM_PIN_SRC_BIT_ERR_CAN_A      = 0x12
//     KM_PIN_SRC_FORM_ERR_CAN_A     = 0x13
//     KM_PIN_SRC_STUFF_ERR_CAN_A    = 0x14
//     KM_PIN_SRC_OTHER_ERR_CAN_A    = 0x15
//     KM_PIN_SRC_ALL_ERR_CAN_B      = 0x21
//     KM_PIN_SRC_BIT_ERR_CAN_B      = 0x22
//     KM_PIN_SRC_FORM_ERR_CAN_B     = 0x23
//     KM_PIN_SRC_STUFF_ERR_CAN_B    = 0x24
//     KM_PIN_SRC_OTHER_ERR_CAN_B    = 0x25
//     KM_PIN_SRC_ALL_ERR_CAN_BOTH   = 0x31
//     KM_PIN_SRC_BIT_ERR_CAN_BOTH   = 0x32
//     KM_PIN_SRC_FORM_ERR_CAN_BOTH  = 0x33
//     KM_PIN_SRC_STUFF_ERR_CAN_BOTH = 0x34
//     KM_PIN_SRC_OTHER_ERR_CAN_BOTH = 0x35

/*
 * Configure a GPIO pin to act as an output. The return value
 * of the function is a status code.
 */
public static int km_gpio_config_out (
    int   komodo,
    byte  pin_number,
    byte  drive,
    byte  source
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_gpio_config_out(komodo, pin_number, drive, source);
}

/*
 * Set the value of any GPIO pins configured as software controlled
 * outputs. The return value of the function is a status code.
 */
public static int km_gpio_set (
    int   komodo,
    byte  value,
    byte  mask
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_gpio_set(komodo, value, mask);
}

/* Returns the current values of all GPIO pins. */
public static int km_gpio_get (
    int  komodo
)
{
    if (!KM_LIBRARY_LOADED) return (int)km_status_t.KM_INCOMPATIBLE_LIBRARY;
    return net_km_gpio_get(komodo);
}


/*=========================================================================
| NATIVE DLL BINDINGS
 ========================================================================*/
[DllImport ("komodo")]
private static extern int net_km_find_devices (int num_ports, [Out] ushort[] ports);

[DllImport ("komodo")]
private static extern int net_km_find_devices_ext (int num_ports, [Out] ushort[] ports, int num_ids, [Out] uint[] unique_ids);

[DllImport ("komodo")]
private static extern int net_km_open (int port_number);

[DllImport ("komodo")]
private static extern int net_km_open_ext (int port_number, ref KomodoExt km_ext);

[DllImport ("komodo")]
private static extern int net_km_close (int komodo);

[DllImport ("komodo")]
private static extern int net_km_port (int komodo);

[DllImport ("komodo")]
private static extern int net_km_features (int komodo);

[DllImport ("komodo")]
private static extern uint net_km_unique_id (int komodo);

[DllImport ("komodo")]
private static extern IntPtr net_km_status_string (int status);

[DllImport ("komodo")]
private static extern int net_km_version (int komodo, ref KomodoVersion version);

[DllImport ("komodo")]
private static extern uint net_km_sleep_ms (uint milliseconds);

[DllImport ("komodo")]
private static extern int net_km_acquire (int komodo, uint features);

[DllImport ("komodo")]
private static extern int net_km_release (int komodo, uint features);

[DllImport ("komodo")]
private static extern int net_km_timeout (int komodo, uint timeout_ms);

[DllImport ("komodo")]
private static extern int net_km_can_query_bus_state (int komodo, km_can_ch_t channel, ref byte bus_state, ref byte rx_error, ref byte tx_error);

[DllImport ("komodo")]
private static extern int net_km_latency (int komodo, uint latency_ms);

[DllImport ("komodo")]
private static extern int net_km_can_configure (int komodo, uint config);

[DllImport ("komodo")]
private static extern int net_km_can_bus_timeout (int komodo, km_can_ch_t channel, ushort timeout_ms);

[DllImport ("komodo")]
private static extern int net_km_can_bitrate (int komodo, km_can_ch_t channel, uint bitrate_hz);

[DllImport ("komodo")]
private static extern int net_km_can_auto_bitrate (int komodo, km_can_ch_t channel);

[DllImport ("komodo")]
private static extern int net_km_can_auto_bitrate_ext (int komodo, km_can_ch_t channel, uint num_bitrates_hz, [In] uint[] bitrates_hz);

[DllImport ("komodo")]
private static extern int net_km_get_samplerate (int komodo);

[DllImport ("komodo")]
private static extern int net_km_can_target_power (int komodo, km_can_ch_t channel, km_power_t power);

[DllImport ("komodo")]
private static extern int net_km_enable (int komodo);

[DllImport ("komodo")]
private static extern int net_km_disable (int komodo);

[DllImport ("komodo")]
private static extern int net_km_can_read (int komodo, ref km_can_info_t info, ref km_can_packet_t pkt, int num_bytes, [Out] byte[] data);

[DllImport ("komodo")]
private static extern int net_km_can_async_submit (int komodo, km_can_ch_t channel, byte flags, ref km_can_packet_t pkt, int num_bytes, [In] byte[] data);

[DllImport ("komodo")]
private static extern int net_km_can_async_collect (int komodo, uint timeout_ms, ref uint arbitration_count);

[DllImport ("komodo")]
private static extern int net_km_can_write (int komodo, km_can_ch_t channel, byte flags, ref km_can_packet_t pkt, int num_bytes, [In] byte[] data, ref uint arbitration_count);

[DllImport ("komodo")]
private static extern int net_km_gpio_config_in (int komodo, byte pin_number, byte bias, byte trigger);

[DllImport ("komodo")]
private static extern int net_km_gpio_config_out (int komodo, byte pin_number, byte drive, byte source);

[DllImport ("komodo")]
private static extern int net_km_gpio_set (int komodo, byte value, byte mask);

[DllImport ("komodo")]
private static extern int net_km_gpio_get (int komodo);


} // class KomodoApi

} // namespace TotalPhase
