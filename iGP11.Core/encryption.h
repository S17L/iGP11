#pragma once

#define ENCRYPTION_KEY 39

static char EncryptCharacter(const char character, int index) {
    return character ^ (ENCRYPTION_KEY + index);
}

template <int... Pack> struct IndexList {};

template <typename IndexList, int Right> struct Append;

template <int... Left, int Right> struct Append<IndexList<Left...>, Right> {
    typedef IndexList<Left..., Right> result;
};

template <int N> struct ConstructIndexList {
    typedef typename Append<typename ConstructIndexList<N - 1>::result, N - 1>::result result;
};

template <> struct ConstructIndexList<0> {
    typedef IndexList<> result;
};

template <typename IndexList> class XorString;

template <int... Index> class XorString<IndexList<Index...>> {
private:
    bool _encrypted = true;
    char _value[sizeof...(Index) + 1];
public:
    __forceinline constexpr XorString(const char* const string)
        : _value { EncryptCharacter(string[Index], Index)... } {}
    char* decrypt() {
        if (_encrypted) {
            for (int t = 0; t < sizeof...(Index); t++) {
                _value[t] = _value[t] ^ (ENCRYPTION_KEY + t);
            }

            _value[sizeof...(Index)] = '\0';
            _encrypted = false;
        }

        return _value;
    }
    char* get() {
        return _value;
    }
};

#define CREATE_ENCRYPTED_STRING(x, string) XorString<ConstructIndexList<sizeof(string) - 1>::result> x(string)
#define ENCRYPT_STRING(string) (XorString<ConstructIndexList<sizeof(string) - 1>::result>(string).decrypt())