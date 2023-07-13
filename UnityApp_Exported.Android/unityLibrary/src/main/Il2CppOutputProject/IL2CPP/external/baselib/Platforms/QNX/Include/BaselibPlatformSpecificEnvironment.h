#pragma once

enum { Baselib_SystemSemaphore_PlatformSize = sizeof(void*) * 4 }; // 16 on 32bit and 32 on 64bit

#define MAX_PATH PATH_MAX

#ifndef EXPORTED_SYMBOL
    #define EXPORTED_SYMBOL __attribute__((visibility("default")))
#endif
#ifndef IMPORTED_SYMBOL
    #define IMPORTED_SYMBOL
#endif

#ifndef PLATFORM_FUTEX_NATIVE_SUPPORT
    #define PLATFORM_FUTEX_NATIVE_SUPPORT 0
#endif

// Use PLATFORM_USE_GCC_ATOMIC_CMPXCHG128_PATCH for QNX GCC (version 5.0 and later) 128-bit__atomic_compare_exchange intrinsic (see Baselib_Atomic_Gcc_Patch.h for more details)
#if (!defined(PLATFORM_USE_GCC_ATOMIC_CMPXCHG128_PATCH)) && (__GNUC__ >= 5) && (defined(_AMD64_) || defined(__LP64__) || defined(_WIN64) || defined(_M_ARM64) || (__aarch64__))
    #define PLATFORM_USE_GCC_ATOMIC_CMPXCHG128_PATCH 1
#endif

#ifndef PLATFORM_LLSC_NATIVE_SUPPORT
    #define PLATFORM_LLSC_NATIVE_SUPPORT 0
#endif

#ifdef __cplusplus
extern "C" {
#endif
// From #include <signal.h>
int raise(int sig);
#ifdef __cplusplus
}
#endif

// SIGTRAP from #include <signal.h>
// checked via static assert in platform config.
#define DETAIL_BASELIB_SIGTRAP 5

#define BASELIB_DEBUG_TRAP() raise(DETAIL_BASELIB_SIGTRAP)
