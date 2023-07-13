#pragma once

#include "../../../C/Baselib_Atomic.h"
#include "../../../C/Baselib_Atomic_Macros.h"
#include "Baselib_Atomic_Gcc_Patch.h"

#if COMPILER_GCC && ((__GNUC__ < 4) || (__GNUC__ == 4 && __GNUC_MINOR__ < 7))
#pragma message "GNUC: " PP_STRINGIZE(__GNUC__) " GNUC_MINOR: " PP_STRINGIZE(__GNUC_MINOR__)
#error "GCC is too old and/or missing compatible atomic built-in functions" PP_STRINGIZE(__GNUC__)
#endif

#define detail_intrinsic_relaxed __ATOMIC_RELAXED
#define detail_intrinsic_acquire __ATOMIC_ACQUIRE
#define detail_intrinsic_release __ATOMIC_RELEASE
#define detail_intrinsic_acq_rel __ATOMIC_ACQ_REL
#define detail_intrinsic_seq_cst __ATOMIC_SEQ_CST

// Patch gcc and clang intrinsics to achieve a sequentially consistent barrier.
// As of writing Clang 9, GCC 9 none of them produce a seq cst barrier for load-store operations.
// To fix this we switch load store to be acquire release with a full final barrier.

#define detail_ldst_intrinsic_relaxed detail_intrinsic_relaxed
#define detail_ldst_intrinsic_acquire detail_intrinsic_acquire
#define detail_ldst_intrinsic_release detail_intrinsic_release
#define detail_ldst_intrinsic_acq_rel detail_intrinsic_acq_rel
#define detail_ldst_intrinsic_seq_cst detail_intrinsic_seq_cst

#if defined(__aarch64__)
    #undef detail_ldst_intrinsic_seq_cst
    #define detail_ldst_intrinsic_seq_cst __ATOMIC_ACQ_REL
    #define detail_AARCH64_SEQCST_PATCH_BARRIER_relaxed
    #define detail_AARCH64_SEQCST_PATCH_BARRIER_acquire
    #define detail_AARCH64_SEQCST_PATCH_BARRIER_release
    #define detail_AARCH64_SEQCST_PATCH_BARRIER_acq_rel
    #define detail_AARCH64_SEQCST_PATCH_BARRIER_seq_cst __extension__({__atomic_thread_fence (__ATOMIC_SEQ_CST); });
#else
    #define detail_AARCH64_SEQCST_PATCH_BARRIER_relaxed
    #define detail_AARCH64_SEQCST_PATCH_BARRIER_acquire
    #define detail_AARCH64_SEQCST_PATCH_BARRIER_release
    #define detail_AARCH64_SEQCST_PATCH_BARRIER_acq_rel
    #define detail_AARCH64_SEQCST_PATCH_BARRIER_seq_cst
#endif


// The GCC included with QNX considers a fail order equal or stronger than success order an invalid memory model for __atomic_compare_exchange,
// which otherwise is allowed by the c++ standard.
// We amend this by forcing the failure order to acquire under such circumstance.
#if defined(__QNX__)
#define detail_QNX_CMP_XCHG_BARRIER_ORDER2_PATCH(order1, order2) order2 < order1 ? order2 : order2 == detail_ldst_intrinsic_relaxed ? order2 : detail_ldst_intrinsic_acquire
#else
#define detail_QNX_CMP_XCHG_BARRIER_ORDER2_PATCH(order1, order2) order2
#endif

#define detail_THREAD_FENCE(order, ...)                                                                                     \
static FORCE_INLINE void Baselib_atomic_thread_fence_##order(void)                                                          \
{                                                                                                                           \
    __extension__({__atomic_thread_fence (detail_intrinsic_##order); });                                                    \
}                                                                                                                           \

#define detail_LOAD(op, order, id , bits, int_type, ...)                                                                    \
static FORCE_INLINE void Baselib_atomic_##op##_##id##_##order##_v(const void* obj, void* result)                            \
{                                                                                                                           \
    __extension__({ __atomic_load((int_type*)obj, (int_type*)result, detail_intrinsic_##order); });                         \
}

#define detail_LOAD_NOT_CONST(op, order, id , bits, int_type, ...)                                                          \
static FORCE_INLINE void Baselib_atomic_##op##_##id##_##order##_v(void* obj, void* result)                                  \
{                                                                                                                           \
    __extension__({ __atomic_load((int_type*)obj, (int_type*)result, detail_intrinsic_##order); });                         \
}

#define detail_STORE(op, order, id , bits, int_type, ...)                                                                   \
static FORCE_INLINE void Baselib_atomic_##op##_##id##_##order##_v(void* obj, const void* value)                             \
{                                                                                                                           \
    __extension__({ __atomic_store((int_type*)obj, (int_type*)value, detail_intrinsic_##order); });                         \
}

#define detail_ALU(op, order, id , bits, int_type, ...)                                                                     \
static FORCE_INLINE void Baselib_atomic_##op##_##id##_##order##_v(void* obj, const void* value, void* result)               \
{                                                                                                                           \
    *(int_type*)result = __extension__({ __atomic_##op((int_type*)obj, *(int_type*)value, detail_ldst_intrinsic_##order); });\
    detail_AARCH64_SEQCST_PATCH_BARRIER_##order;                                                                            \
}

#define detail_XCHG(op, order, id , bits, int_type, ...)                                                                    \
static FORCE_INLINE void Baselib_atomic_##op##_##id##_##order##_v(void* obj, const void* value, void* result)               \
{                                                                                                                           \
    __extension__({ __atomic_exchange((int_type*)obj, (int_type*)value, (int_type*)result, detail_ldst_intrinsic_##order); });\
    detail_AARCH64_SEQCST_PATCH_BARRIER_##order;                                                                            \
}

#define detail_CMP_XCHG_WEAK(op, order1, order2, id , bits, int_type, ...)                                                  \
static FORCE_INLINE bool Baselib_atomic_##op##_##id##_##order1##_##order2##_v(void* obj, void* expected, const void* value) \
{                                                                                                                           \
    detail_GCC_CMP_XCHG_128_WEAK_QNX_PATCH(order1, order2, int_type, obj, expected, value);                                 \
    bool result = __extension__({ __atomic_compare_exchange(                                                                \
        (int_type*)obj,                                                                                                     \
        (int_type*)expected,                                                                                                \
        (int_type*)value,                                                                                                   \
        1,                                                                                                                  \
        detail_ldst_intrinsic_##order1,                                                                                     \
        detail_QNX_CMP_XCHG_BARRIER_ORDER2_PATCH(detail_ldst_intrinsic_##order1, detail_ldst_intrinsic_##order2));          \
    });                                                                                                                     \
    if (result) { detail_AARCH64_SEQCST_PATCH_BARRIER_##order1; }                                                           \
    else { detail_AARCH64_SEQCST_PATCH_BARRIER_##order2;}                                                                   \
    return result;                                                                                                          \
}

#define detail_CMP_XCHG_STRONG(op, order1, order2, id , bits, int_type, ...)                                                \
static FORCE_INLINE bool Baselib_atomic_##op##_##id##_##order1##_##order2##_v(void* obj, void* expected, const void* value) \
{                                                                                                                           \
    detail_GCC_CMP_XCHG_128_STRONG_QNX_PATCH(order1, order2, int_type, obj, expected, value);                               \
    bool result =  __extension__ ({ __atomic_compare_exchange(                                                              \
        (int_type*)obj,                                                                                                     \
        (int_type*)expected,                                                                                                \
        (int_type*)value,                                                                                                   \
        0,                                                                                                                  \
        detail_ldst_intrinsic_##order1,                                                                                     \
        detail_QNX_CMP_XCHG_BARRIER_ORDER2_PATCH(detail_ldst_intrinsic_##order1, detail_ldst_intrinsic_##order2));          \
    });                                                                                                                     \
    if (result) { detail_AARCH64_SEQCST_PATCH_BARRIER_##order1; }                                                           \
    else { detail_AARCH64_SEQCST_PATCH_BARRIER_##order2;}                                                                   \
    return result;                                                                                                          \
}

#define detail_NOT_SUPPORTED(...)

Baselib_Atomic_FOR_EACH_MEMORY_ORDER(
    detail_THREAD_FENCE
)

Baselib_Atomic_FOR_EACH_ATOMIC_OP_MEMORY_ORDER_AND_TYPE(
    detail_LOAD,            // load
    detail_STORE,           // store
    detail_ALU,             // add
    detail_ALU,             // and
    detail_ALU,             // or
    detail_ALU,             // xor
    detail_XCHG,            // exchange
    detail_CMP_XCHG_WEAK,   // compare_exchange_weak
    detail_CMP_XCHG_STRONG, // compare_exchange_strong
)

#if PLATFORM_ARCH_64

// 128-bit implementation
// GCC 7.0 and higher does not provide __atomic_load, store or xchg 16b, so we fallback to cmpxchg for those atomic ops.
// For QNX we do this for GCC version 5.0 and higher (incorrect versioning?)
#if PLATFORM_USE_GCC_ATOMIC_CMPXCHG128_PATCH

// QNX GCC < 7.1 erraneously reports uninitialized memory
#pragma GCC diagnostic push
#pragma GCC diagnostic ignored "-Wuninitialized"

#define detail_LOAD_NOT_CONST128(op, order, id, ...)                                                            \
static FORCE_INLINE void Baselib_atomic_##op##_##id##_##order##_v(void* obj, void* result)                      \
{                                                                                                               \
    *((__int128*)result) = 0;                                                                                   \
    Baselib_atomic_compare_exchange_weak_128_##order##_##order##_v((void*)obj, result, result);                 \
}

#define detail_STORE128(op, order, id, ...)                                                                     \
static FORCE_INLINE void Baselib_atomic_##op##_##id##_##order##_v(void* obj, const void* value)                 \
{                                                                                                               \
    __int128 comparand = *((volatile __int128*)obj);                                                            \
    while(!Baselib_atomic_compare_exchange_weak_128_##order##_relaxed_v(obj, &comparand, value));               \
}

#define detail_XCHG128(op, order, id, ...)                                                                      \
static FORCE_INLINE void Baselib_atomic_##op##_##id##_##order##_v(void* obj, const void* value, void* result)   \
{                                                                                                               \
    *((__int128*)result) = *((volatile __int128*)obj);                                                          \
    while(!Baselib_atomic_compare_exchange_weak_128_##order##_relaxed_v(obj, result, value));                   \
}

#pragma GCC diagnostic pop

Baselib_Atomic_FOR_EACH_ATOMIC_OP_AND_MEMORY_ORDER(
    detail_LOAD_NOT_CONST128,   // load
    detail_STORE128,            // store
    detail_NOT_SUPPORTED,       // add
    detail_NOT_SUPPORTED,       // and
    detail_NOT_SUPPORTED,       // or
    detail_NOT_SUPPORTED,       // xor
    detail_XCHG128,             // exchange
    detail_CMP_XCHG_WEAK,       // compare_exchange_weak
    detail_CMP_XCHG_STRONG,     // compare_exchange_strong
    128, 128, __int128          // type information
)

Baselib_Atomic_FOR_EACH_ATOMIC_OP_AND_MEMORY_ORDER(
    detail_LOAD_NOT_CONST128,   // load
    detail_STORE128,            // store
    detail_NOT_SUPPORTED,       // add
    detail_NOT_SUPPORTED,       // and
    detail_NOT_SUPPORTED,       // or
    detail_NOT_SUPPORTED,       // xor
    detail_XCHG128,             // exchange
    detail_CMP_XCHG_WEAK,       // compare_exchange_weak
    detail_CMP_XCHG_STRONG,     // compare_exchange_strong
    ptr2x, 128, __int128        // type information
)

#undef detail_LOAD_NOT_CONST128
#undef detail_STORE128
#undef detail_XCHG128

#else

Baselib_Atomic_FOR_EACH_ATOMIC_OP_AND_MEMORY_ORDER(
    detail_LOAD_NOT_CONST,      // load
    detail_STORE,               // store
    detail_NOT_SUPPORTED,       // add
    detail_NOT_SUPPORTED,       // and
    detail_NOT_SUPPORTED,       // or
    detail_NOT_SUPPORTED,       // xor
    detail_XCHG,                // exchange
    detail_CMP_XCHG_WEAK,       // compare_exchange_weak
    detail_CMP_XCHG_STRONG,     // compare_exchange_strong
    128, 128, __int128          // type information
)

Baselib_Atomic_FOR_EACH_ATOMIC_OP_AND_MEMORY_ORDER(
    detail_LOAD_NOT_CONST,      // load
    detail_STORE,               // store
    detail_NOT_SUPPORTED,       // add
    detail_NOT_SUPPORTED,       // and
    detail_NOT_SUPPORTED,       // or
    detail_NOT_SUPPORTED,       // xor
    detail_XCHG,                // exchange
    detail_CMP_XCHG_WEAK,       // compare_exchange_weak
    detail_CMP_XCHG_STRONG,     // compare_exchange_strong
    ptr2x, 128, __int128        // type information
)

#endif // PLATFORM_USE_GCC_ATOMIC_CMPXCHG128_PATCH

#else

Baselib_Atomic_FOR_EACH_ATOMIC_OP_AND_MEMORY_ORDER(
    detail_LOAD_NOT_CONST,      // load
    detail_STORE,               // store
    detail_NOT_SUPPORTED,       // add
    detail_NOT_SUPPORTED,       // and
    detail_NOT_SUPPORTED,       // or
    detail_NOT_SUPPORTED,       // xor
    detail_XCHG,                // exchange
    detail_CMP_XCHG_WEAK,       // compare_exchange_weak
    detail_CMP_XCHG_STRONG,     // compare_exchange_strong
    ptr2x, 64, int64_t          // type information
)

#endif // PLATFORM_ARCH_64

#undef detail_intrinsic_relaxed
#undef detail_intrinsic_acquire
#undef detail_intrinsic_release
#undef detail_intrinsic_acq_rel
#undef detail_intrinsic_seq_cst

#undef detail_THREAD_FENCE
#undef detail_LOAD
#undef detail_LOAD_NOT_CONST
#undef detail_STORE
#undef detail_ALU
#undef detail_XCHG
#undef detail_CMP_XCHG_WEAK
#undef detail_CMP_XCHG_STRONG
#undef detail_NOT_SUPPORTED

#include "Baselib_Atomic_Gcc_Patch_PostInclude.h"
