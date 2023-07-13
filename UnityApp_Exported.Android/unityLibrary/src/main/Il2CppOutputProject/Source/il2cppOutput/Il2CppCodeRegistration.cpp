#include "pch-cpp.hpp"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif







IL2CPP_EXTERN_C const Il2CppMethodPointer g_ReversePInvokeWrapperPointers[];
IL2CPP_EXTERN_C const Il2CppMethodPointer g_Il2CppGenericMethodPointers[];
IL2CPP_EXTERN_C const Il2CppMethodPointer g_Il2CppGenericAdjustorThunks[];
IL2CPP_EXTERN_C const InvokerMethod g_Il2CppInvokerPointers[];
IL2CPP_EXTERN_C const Il2CppMethodPointer g_UnresolvedVirtualMethodPointers[];
IL2CPP_EXTERN_C const Il2CppMethodPointer g_UnresolvedInstanceMethodPointers[];
IL2CPP_EXTERN_C const Il2CppMethodPointer g_UnresolvedStaticMethodPointers[];
IL2CPP_EXTERN_C Il2CppInteropData g_Il2CppInteropData[];
IL2CPP_EXTERN_C_CONST Il2CppCodeGenModule g_AssemblyU2DCSharp_CodeGenModule;
IL2CPP_EXTERN_C_CONST Il2CppCodeGenModule g_Mono_Security_CodeGenModule;
IL2CPP_EXTERN_C_CONST Il2CppCodeGenModule g_System_CodeGenModule;
IL2CPP_EXTERN_C_CONST Il2CppCodeGenModule g_System_Configuration_CodeGenModule;
IL2CPP_EXTERN_C_CONST Il2CppCodeGenModule g_System_Core_CodeGenModule;
IL2CPP_EXTERN_C_CONST Il2CppCodeGenModule g_System_Xml_CodeGenModule;
IL2CPP_EXTERN_C_CONST Il2CppCodeGenModule g_UnityEngine_AndroidJNIModule_CodeGenModule;
IL2CPP_EXTERN_C_CONST Il2CppCodeGenModule g_UnityEngine_AudioModule_CodeGenModule;
IL2CPP_EXTERN_C_CONST Il2CppCodeGenModule g_UnityEngine_CodeGenModule;
IL2CPP_EXTERN_C_CONST Il2CppCodeGenModule g_UnityEngine_CoreModule_CodeGenModule;
IL2CPP_EXTERN_C_CONST Il2CppCodeGenModule g_UnityEngine_InputLegacyModule_CodeGenModule;
IL2CPP_EXTERN_C_CONST Il2CppCodeGenModule g_UnityEngine_PhysicsModule_CodeGenModule;
IL2CPP_EXTERN_C_CONST Il2CppCodeGenModule g_UnityEngine_PropertiesModule_CodeGenModule;
IL2CPP_EXTERN_C_CONST Il2CppCodeGenModule g_UnityEngine_SharedInternalsModule_CodeGenModule;
IL2CPP_EXTERN_C_CONST Il2CppCodeGenModule g___Generated_CodeGenModule;
IL2CPP_EXTERN_C_CONST Il2CppCodeGenModule g_mscorlib_CodeGenModule;
IL2CPP_EXTERN_C const Il2CppCodeGenModule* g_CodeGenModules[];
const Il2CppCodeGenModule* g_CodeGenModules[16] = 
{
	(&g_AssemblyU2DCSharp_CodeGenModule),
	(&g_Mono_Security_CodeGenModule),
	(&g_System_CodeGenModule),
	(&g_System_Configuration_CodeGenModule),
	(&g_System_Core_CodeGenModule),
	(&g_System_Xml_CodeGenModule),
	(&g_UnityEngine_AndroidJNIModule_CodeGenModule),
	(&g_UnityEngine_AudioModule_CodeGenModule),
	(&g_UnityEngine_CodeGenModule),
	(&g_UnityEngine_CoreModule_CodeGenModule),
	(&g_UnityEngine_InputLegacyModule_CodeGenModule),
	(&g_UnityEngine_PhysicsModule_CodeGenModule),
	(&g_UnityEngine_PropertiesModule_CodeGenModule),
	(&g_UnityEngine_SharedInternalsModule_CodeGenModule),
	(&g___Generated_CodeGenModule),
	(&g_mscorlib_CodeGenModule),
};
IL2CPP_EXTERN_C const Il2CppCodeRegistration g_CodeRegistration;
const Il2CppCodeRegistration g_CodeRegistration = 
{
	2,
	g_ReversePInvokeWrapperPointers,
	5195,
	g_Il2CppGenericMethodPointers,
	g_Il2CppGenericAdjustorThunks,
	2321,
	g_Il2CppInvokerPointers,
	243,
	g_UnresolvedVirtualMethodPointers,
	g_UnresolvedInstanceMethodPointers,
	g_UnresolvedStaticMethodPointers,
	149,
	g_Il2CppInteropData,
	0,
	NULL,
	16,
	g_CodeGenModules,
};
IL2CPP_EXTERN_C_CONST Il2CppMetadataRegistration g_MetadataRegistration;
static const Il2CppCodeGenOptions s_Il2CppCodeGenOptions = 
{
	true,
	7,
	1,
};
void s_Il2CppCodegenRegistration()
{
	il2cpp_codegen_register (&g_CodeRegistration, &g_MetadataRegistration, &s_Il2CppCodeGenOptions);
}
#if RUNTIME_IL2CPP
typedef void (*CodegenRegistrationFunction)();
CodegenRegistrationFunction g_CodegenRegistration = s_Il2CppCodegenRegistration;
#endif
