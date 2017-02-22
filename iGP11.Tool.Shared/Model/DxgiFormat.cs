using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model
{
    public enum DxgiFormat
    {
        [ComponentName("R32G32B32A32_TYPELESS", false)]
        R32G32B32A32Typeless = 1,

        [ComponentName("R32G32B32A32_FLOAT", false)]
        R32G32B32A32Float = 2,

        [ComponentName("R32G32B32A32_UINT", false)]
        R32G32B32A32Uint = 3,

        [ComponentName("R32G32B32A32_SINT", false)]
        R32G32B32A32Sint = 4,

        [ComponentName("R32G32B32_TYPELESS", false)]
        R32G32B32Typeless = 5,

        [ComponentName("R32G32B32_FLOAT", false)]
        R32G32B32Float = 6,

        [ComponentName("R32G32B32_UINT", false)]
        R32G32B32Uint = 7,

        [ComponentName("R32G32B32_SINT", false)]
        R32G32B32Sint = 8,

        [ComponentName("R16G16B16A16_TYPELESS", false)]
        R16G16B16A16Typeless = 9,

        [ComponentName("R16G16B16A16_FLOAT", false)]
        R16G16B16A16Float = 10,

        [ComponentName("R16G16B16A16_UNORM", false)]
        R16G16B16A16Unorm = 11,

        [ComponentName("R16G16B16A16_UINT", false)]
        R16G16B16A16Uint = 12,

        [ComponentName("R16G16B16A16_SNORM", false)]
        R16G16B16A16Snorm = 13,

        [ComponentName("R16G16B16A16_SINT", false)]
        R16G16B16A16Sint = 14,

        [ComponentName("R32G32_TYPELESS", false)]
        R32G32Typeless = 15,

        [ComponentName("R32G32_FLOAT", false)]
        R32G32Float = 16,

        [ComponentName("R32G32_UINT", false)]
        R32G32Uint = 17,

        [ComponentName("R32G32_SINT", false)]
        R32G32Sint = 18,

        [ComponentName("R32G8X24_TYPELESS", false)]
        R32G8X24Typeless = 19,

        [ComponentName("D32_FLOAT_S8X24_UINT", false)]
        D32FloatS8X24Uint = 20,

        [ComponentName("R32_FLOAT_X8X24_TYPELESS", false)]
        R32FloatX8X24Typeless = 21,

        [ComponentName("X32_TYPELESS_G8X24_UINT", false)]
        X32TypelessG8X24Uint = 22,

        [ComponentName("R10G10B10A2_TYPELESS", false)]
        R10G10B10A2Typeless = 23,

        [ComponentName("R10G10B10A2_UNORM", false)]
        R10G10B10A2Unorm = 24,

        [ComponentName("R10G10B10A2_UINT", false)]
        R10G10B10A2Uint = 25,

        [ComponentName("R11G11B10_FLOAT", false)]
        R11G11B10Float = 26,

        [ComponentName("R8G8B8A8_TYPELESS", false)]
        R8G8B8A8Typeless = 27,

        [ComponentName("R8G8B8A8_UNORM", false)]
        [Editable]
        R8G8B8A8Unorm = 28,

        [ComponentName("R8G8B8A8_UNORM_SRGB", false)]
        R8G8B8A8UnormSrgb = 29,

        [ComponentName("R8G8B8A8_UINT", false)]
        R8G8B8A8Uint = 30,

        [ComponentName("R8G8B8A8_SNORM", false)]
        R8G8B8A8Snorm = 31,

        [ComponentName("R8G8B8A8_SINT", false)]
        R8G8B8A8Sint = 32,

        [ComponentName("R16G16_TYPELESS", false)]
        R16G16Typeless = 33,

        [ComponentName("R16G16_FLOAT", false)]
        R16G16Float = 34,

        [ComponentName("R16G16_UNORM", false)]
        R16G16Unorm = 35,

        [ComponentName("R16G16_UINT", false)]
        R16G16Uint = 36,

        [ComponentName("R16G16_SNORM", false)]
        R16G16Snorm = 37,

        [ComponentName("R16G16_SINT", false)]
        R16G16Sint = 38,

        [ComponentName("R32_TYPELESS", false)]
        R32Typeless = 39,

        [ComponentName("D32_FLOAT", false)]
        D32Float = 40,

        [ComponentName("R32_FLOAT", false)]
        R32Float = 41,

        [ComponentName("R32_UINT", false)]
        R32Uint = 42,

        [ComponentName("R32_SINT", false)]
        R32Sint = 43,

        [ComponentName("R24G8_TYPELESS", false)]
        R24G8Typeless = 44,

        [ComponentName("D24_UNORM_S8_UINT", false)]
        D24UnormS8Uint = 45,

        [ComponentName("R24_UNORM_X8_TYPELESS", false)]
        R24UnormX8Typeless = 46,

        [ComponentName("X24_TYPELESS_G8_UINT", false)]
        X24TypelessG8Uint = 47,

        [ComponentName("R8G8_TYPELESS", false)]
        R8G8Typeless = 48,

        [ComponentName("R8G8_UNORM", false)]
        R8G8Unorm = 49,

        [ComponentName("R8G8_UINT", false)]
        R8G8Uint = 50,

        [ComponentName("R8G8_SNORM", false)]
        R8G8Snorm = 51,

        [ComponentName("R8G8_SINT", false)]
        R8G8Sint = 52,

        [ComponentName("R16_TYPELESS", false)]
        R16Typeless = 53,

        [ComponentName("R16_FLOAT", false)]
        R16Float = 54,

        [ComponentName("D16_UNORM", false)]
        D16Unorm = 55,

        [ComponentName("R16_UNORM", false)]
        R16Unorm = 56,

        [ComponentName("R16_SNORM", false)]
        R16Snorm = 58,

        [ComponentName("R8_TYPELESS", false)]
        R8Typeless = 60,

        [ComponentName("R8_UNORM", false)]
        R8Unorm = 61,

        [ComponentName("R8_UINT", false)]
        R8Uint = 62,

        [ComponentName("R8_SNORM", false)]
        R8Snorm = 63,

        [ComponentName("R8_SINT", false)]
        R8Sint = 64,

        [ComponentName("A8_UNORM", false)]
        A8Unorm = 65,

        [ComponentName("R1_UNORM", false)]
        R1Unorm = 66,

        [ComponentName("R9G9B9E5_SHAREDEXP", false)]
        R9G9B9E5Sharedexp = 67,

        [ComponentName("R8G8_B8G8_UNORM", false)]
        R8G8B8G8Unorm = 68,

        [ComponentName("G8R8_G8B8_UNORM", false)]
        G8R8G8B8Unorm = 69,

        [ComponentName("BC1_TYPELESS", false)]
        Bc1Typeless = 70,

        [ComponentName("BC1_UNORM", false)]
        [Editable]
        Bc1Unorm = 71,

        [ComponentName("BC1_UNORM_SRGB", false)]
        Bc1UnormSrgb = 72,

        [ComponentName("BC2_TYPELESS", false)]
        Bc2Typeless = 73,

        [ComponentName("BC2_UNORM", false)]
        [Editable]
        Bc2Unorm = 74,

        [ComponentName("BC2_UNORM_SRGB", false)]
        Bc2UnormSrgb = 75,

        [ComponentName("BC3_TYPELESS", false)]
        Bc3Typeless = 76,

        [ComponentName("BC3_UNORM", false)]
        [Editable]
        Bc3Unorm = 77,

        [ComponentName("BC3_UNORM_SRGB", false)]
        Bc3UnormSrgb = 78,

        [ComponentName("BC4_TYPELESS", false)]
        Bc4Typeless = 79,

        [ComponentName("BC4_UNORM", false)]
        Bc4Unorm = 80,

        [ComponentName("BC4_SNORM", false)]
        Bc4Snorm = 81,

        [ComponentName("BC5_TYPELESS", false)]
        Bc5Typeless = 82,

        [ComponentName("BC5_UNORM", false)]
        Bc5Unorm = 83,

        [ComponentName("BC5_SNORM", false)]
        Bc5Snorm = 84,

        [ComponentName("B5G6R5_UNORM", false)]
        B5G6R5Unorm = 85,

        [ComponentName("B5G5R5A1_UNORM", false)]
        B5G5R5A1Unorm = 86,

        [ComponentName("B8G8R8A8_UNORM", false)]
        [Editable]
        B8G8R8A8Unorm = 87,

        [ComponentName("B8G8R8X8_UNORM", false)]
        B8G8R8X8Unorm = 88,

        [ComponentName("R10G10B10_XR_BIAS_A2_UNORM", false)]
        R10G10B10XrBiasA2Unorm = 89,

        [ComponentName("B8G8R8A8_TYPELESS", false)]
        B8G8R8A8Typeless = 90,

        [ComponentName("B8G8R8A8_UNORM_SRGB", false)]
        B8G8R8A8UnormSrgb = 91,

        [ComponentName("B8G8R8X8_TYPELESS", false)]
        B8G8R8X8Typeless = 92,

        [ComponentName("B8G8R8X8_UNORM_SRGB", false)]
        B8G8R8X8UnormSrgb = 93,

        [ComponentName("BC6H_TYPELESS", false)]
        Bc6HTypeless = 94,

        [ComponentName("BC6H_UF16", false)]
        Bc6HUf16 = 95,

        [ComponentName("BC6H_SF16", false)]
        Bc6HSf16 = 96,

        [ComponentName("BC7_TYPELESS", false)]
        Bc7Typeless = 97,

        [ComponentName("BC7_UNORM", false)]
        Bc7Unorm = 98,

        [ComponentName("BC7_UNORM_SRGB", false)]
        Bc7UnormSrgb = 99,

        [ComponentName("AYUV", false)]
        Ayuv = 100,

        [ComponentName("Y410", false)]
        Y410 = 101,

        [ComponentName("Y416", false)]
        Y416 = 102,

        [ComponentName("NV12", false)]
        Nv12 = 103,

        [ComponentName("P010", false)]
        P010 = 104,

        [ComponentName("P016", false)]
        P016 = 105,

        [ComponentName("420_OPAQUE", false)]
        F420Opaque = 106,

        [ComponentName("YUY2", false)]
        Yuy2 = 107,

        [ComponentName("Y210", false)]
        Y210 = 108,

        [ComponentName("Y216", false)]
        Y216 = 109,

        [ComponentName("NV11", false)]
        Nv11 = 110,

        [ComponentName("AI44", false)]
        Ai44 = 111,

        [ComponentName("IA44", false)]
        Ia44 = 112,

        [ComponentName("P8", false)]
        P8 = 113,

        [ComponentName("A8P8", false)]
        A8P8 = 114,

        [ComponentName("B4G4R4A4_UNORM", false)]
        B4G4R4A4Unorm = 115,

        [ComponentName("P208", false)]
        P208 = 130,

        [ComponentName("V208", false)]
        V208 = 131,

        [ComponentName("V408", false)]
        V408 = 132
    }
}