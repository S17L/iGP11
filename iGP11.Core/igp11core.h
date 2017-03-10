#pragma once

#include "stdafx.h"
#include "encryption.h"

namespace core {
    enum class ActivationStatus {
        notretrievable = 0,
        notrunning = 1,
        running = 2,
        pluginloaded = 3,
        pluginactivationpending = 4,
        pluginactivated = 5,
        pluginactivationfailed = 6
    };

    enum class PluginType {
        direct3d11 = 0
    };

    enum class Direct3D11ProfileType {
        generic = 0,
        darksouls2 = 1,
        darksouls3 = 2,
        fallout4 = 3
    };

    enum class RenderingMode {
        alpha = 0,
        depthbuffer = 1,
        effects = 2,
        luminescence = 3,
        none = 4
    };

    enum class RequestType {
        getactivationstatus = 0,
        getproxysettings = 1,
        updateproxysettings = 2
    };

    enum class TextureDetailLevel {
        default = 0,
        lowest = 1,
        low = 2,
        medium = 3,
        high = 4,
        highest = 5
    };

    enum class TextureOverrideMode {
        none = 0,
        dumping = 1,
        override = 2
    };

    std::string toString(std::wstring text);
    std::wstring toWString(std::string text);

    inline std::string join(const std::string &delimiter, const std::list<std::string> collection) {
        if (collection.empty()) {
            return std::string();
        }

        std::ostringstream stream;
        auto text = collection.begin();
        auto last = collection.end();
        --last;

        while (text != collection.end()) {
            stream << *text;
            if (text != last) {
                stream << delimiter;
            }

            ++text;
        }

        return stream.str();
    }

    inline bool startsWith(std::string const &text, std::string const &begining) {
        if (text.length() >= begining.length()) {
            return (0 == text.compare(0, begining.length(), begining));
        }
        else {
            return false;
        }
    }

    inline bool endsWith(std::string const &text, std::string const &ending) {
        if (text.length() >= ending.length()) {
            return (0 == text.compare(text.length() - ending.length(), ending.length(), ending));
        }
        else {
            return false;
        }
    }

    inline void toLower(std::string &text) {
        std::transform(text.begin(), text.end(), text.begin(), ::tolower);
    }

    inline void ltrim(std::string &text) {
        text.erase(text.begin(), std::find_if(text.begin(), text.end(), std::not1(std::ptr_fun<int, int>(std::isspace))));
    }

    inline void rtrim(std::string &text) {
        text.erase(std::find_if(text.rbegin(), text.rend(), std::not1(std::ptr_fun<int, int>(std::isspace))).base(), text.end());
    }

    inline void trim(std::string &text) {
        ltrim(text);
        rtrim(text);
    }

    inline std::vector<std::string> split(std::string const &text) {
        std::istringstream stream(text);
        std::vector<std::string> collection(
            (std::istream_iterator<std::string>(stream)),
            std::istream_iterator<std::string>());

        return collection;
    }

    inline bool containsStartAt(const std::string &first, const std::string &second, uint64_t startAt = 0) {
        if (first.length() <= startAt
            || first.length() - startAt < second.length()) {
            return false;
        }

        for (uint64_t i = startAt, j = 0; i < second.length(); i++, j++) {
            if (first[i] != second[j]) {
                return false;
            }
        }

        return true;
    }

    inline bool isEqual(const std::string &first, const std::string &second) {
        if (first.length() != second.length()) {
            return false;
        }

        for (uint64_t i = 0; i < first.length(); i++) {
            if (first[i] != second[i]) {
                return false;
            }
        }

        return true;
    }

    inline bool isEqualIgnoreCase(const std::string &first, const std::string &second) {
        std::string a = first;
        std::string b = second;
        toLower(a);
        toLower(b);

        return isEqual(a, b);
    }

    template <typename TEntry>
    inline std::string toDefaultString(const TEntry& entry) {
        std::ostringstream stream;
        stream << entry;

        return stream.str();
    }

    inline std::string toString(int number, int padding = 10) {
        std::stringstream stream;
        stream << std::setw(padding) << std::setfill('0') << number;

        return stream.str();
    }

    inline int toInt(std::string number) {
        std::string::size_type size;

        return std::stoi(number, &size);
    }

    template<typename ... Args>
    std::string stringFormat(const std::string &format, Args ... args) {
        size_t size = _snprintf(nullptr, 0, format.c_str(), args ...) + 1;
        std::unique_ptr<char[]> buffer(new char[size]);
        _snprintf(buffer.get(), size, format.c_str(), args ...);

        return std::string(buffer.get(), buffer.get() + size - 1);
    }

    template <typename TEntry>
    class Nullable {
    private:
        TEntry _value;
        bool _hasValue;
    public:
        Nullable()
            : _hasValue(false) {}
        Nullable(TEntry value, bool hasValue = true)
            : _value(value), _hasValue(hasValue) {}
        const TEntry* operator->() const {
            return &_value;
        }
        TEntry get() {
            return _value;
        }
        bool hasValue() const {
            return _hasValue;
        }
        void set(TEntry value, bool hasValue = true) {
            _value = value;
            _hasValue = hasValue;
        }
        void set(const Nullable<TEntry> &entry) {
            _value = entry._value;
            _hasValue = entry._hasValue;
        }
    };

    namespace dto {
        struct BokehDoF {
            bool isEnabled = false;
            bool isChromaticAberrationEnabled = false;
            float chromaticAberrationFringe = 0;
            bool isPreservingShape = false;
            unsigned int shapeSize = 0;
            float shapeStrength = 0;
            float shapeRotation = 0;
            bool isBlurEnabled = false;
            float blurStrength = 0;
            float depthMinimum = 0;
            float depthMaximum = 0;
            float depthRateGain = 0;
            float luminescenceMinimum = 0;
            float luminescenceMaximum = 0;
            float luminescenceRateGain = 0;
        };

        struct Command {
            int id;
            std::string data;
        };

        struct DepthBuffer {
            float linearZNear = 0;
            float linearZFar = 0;
            bool isLimitEnabled = false;
            float depthMinimum = 0;
            float depthMaximum = 0;
        };

        struct Textures {
            TextureDetailLevel detailLevel;
            TextureOverrideMode overrideMode;
            std::string dumpingPath;
            std::string overridePath;
        };

        struct Direct3D11PluginSettings {
            Direct3D11ProfileType profileType;
            RenderingMode renderingMode;
        };

        struct LumaSharpen {
            bool isEnabled = false;
            float sharpeningStrength;
            float sharpeningClamp;
            float offsetBias;
        };

        struct Tonemap {
            bool isEnabled = false;
            float gamma = 0;
            float exposure = 0;
            float saturation = 0;
            float bleach = 0;
            float defog = 0;
            float defogRedChannelLoss = 0;
            float defogGreenChannelLoss = 0;
            float defogBlueChannelLoss = 0;
        };

        struct Vibrance {
            bool isEnabled = false;
            float strength = 0;
            float redChannelStrength = 0;
            float greenChannelStrength = 0;
            float blueChannelStrength = 0;
        };

        struct Direct3D11Settings {
            BokehDoF bokehDoF;
            DepthBuffer depthBuffer;
            Direct3D11PluginSettings pluginSettings;
            LumaSharpen lumaSharpen;
            Textures textures;
            Tonemap tonemap;
            Vibrance vibrance;
        };

        struct GameSettings {
            std::string gameFilePath;
            std::string proxyDirectoryPath;
            std::string logsDirectoryPath;
            core::PluginType pluginType;
            std::string communicationAddress;
            unsigned short communicationPort;
            std::string direct3D11PluginPath;
            Direct3D11Settings direct3D11Settings;
        };

        struct ProxySettings {
            std::string gameFilePath;
            std::string proxyDirectoryPath;
            std::string logsDirectoryPath;
            PluginType pluginType;
            ActivationStatus activationStatus;
            Direct3D11Settings direct3D11Settings;
        };

        struct UpdateProxySettings {
            PluginType pluginType;
            Direct3D11Settings direct3D11Settings;
        };

        struct ProcessDetail {
            unsigned long id;
            std::string path;
        };
    }

    class IGameSettingsRepository {
    public:
        ~IGameSettingsRepository() {}
        virtual dto::GameSettings load() = 0;
        virtual void update(dto::Direct3D11Settings settings) = 0;
    };

    class IProcessService {
    public:
        ~IProcessService() {}
        virtual int adjustPrivileges() = 0;
        virtual dto::ProcessDetail getCurrentProcessDetail() = 0;
        virtual dto::ProcessDetail getProcessDetail(unsigned long id) = 0;
        virtual unsigned long getProcessByName(const std::string &applicationFilePath) = 0;
        virtual bool hasLoadedLibrary(const std::string &applicationFilePath, const std::string &libraryFilePath) = 0;
        virtual unsigned long inject(const std::string &applicationFilePath, const std::string &libraryFilePath) = 0;
    };

    class ISettingsService {
    public:
        ~ISettingsService() {}
        virtual core::dto::GameSettings getSettings() = 0;
    };

    class IPlugin {
    public:
        virtual ~IPlugin() {}
        virtual ActivationStatus getActivationStatus() = 0;
    };

    class IDirect3D11Plugin : public IPlugin {
    public:
        virtual ~IDirect3D11Plugin() {}
        virtual dto::Direct3D11Settings getSettings() = 0;
        virtual bool update(dto::Direct3D11Settings settings) = 0;
    };

    class IPluginLoader {
    public:
        virtual ~IPluginLoader() {}
        virtual std::string getName() = 0;
        virtual PluginType getType() = 0;
        virtual bool load() = 0;
        virtual bool start() = 0;
        virtual bool stop() = 0;
        virtual IPlugin* getPlugin() = 0;
    };

    class IDirect3D11PluginLoader : public IPluginLoader {
    public:
        virtual ~IDirect3D11PluginLoader() {}
        virtual IDirect3D11Plugin* getDirect3D11Plugin() = 0;
    };

    class TextureProfile {
    public:
        std::string mapFrom;
        Nullable<bool> forceSrgb;
        Nullable<std::string> mapTo;
        TextureProfile()
            : forceSrgb(false, false) {}
        bool isFor(const std::string &mapFrom) {
            return this->mapFrom.compare(mapFrom) == 0;
        }
    };

    class ITextureCacheVisitor {
    public:
        virtual ~ITextureCacheVisitor() {}
        virtual void visit(const TextureProfile &profile) = 0;
    };

    class ITextureCache {
    public:
        virtual ~ITextureCache() {}
        virtual size_t getCount() = 0;
        virtual bool has(const std::string &id) = 0;
        virtual void accept(ITextureCacheVisitor &visitor) = 0;
        virtual std::shared_ptr<TextureProfile> find(const std::string &id) = 0;
        virtual void merge(TextureProfile &profile) = 0;
    };

    class ITextureCacheFactory {
    public:
        virtual ~ITextureCacheFactory() {}
        virtual std::shared_ptr<ITextureCache> createFromDirectory(std::string directoryPath) = 0;
    };

    class IFrameCounter {
    public:
        virtual ~IFrameCounter() {}
        virtual unsigned int getAverageCount() const = 0;
        virtual unsigned int getTotalCount() const = 0;
        virtual bool nextFrame() = 0;
    };

    class IResourceProvider {
    public:
        virtual ~IResourceProvider() {}
        virtual std::string get(const std::string &key) = 0;
    };

    class ISerializer {
    public:
        virtual ~ISerializer() {}
        virtual core::dto::Command deserializeCommand(const std::string &value) = 0;
        virtual core::dto::GameSettings deserializeSettings(const std::string &value) = 0;
        virtual core::dto::Direct3D11Settings deserializeDirect3D11Settings(const std::string &value) = 0;
        virtual core::dto::UpdateProxySettings deserializeUpdateProxySettings(const std::string &value) = 0;
        virtual std::string serialize(core::dto::ProxySettings data) = 0;
        virtual std::string serialize(core::dto::Direct3D11Settings data) = 0;
    };

    namespace communication {
        class ICommandHandlingPolicy {
        public:
            virtual ~ICommandHandlingPolicy() {}
            virtual bool isAppliciable(const int &id) = 0;
            virtual std::string handle(const std::string &data) = 0;
        };

        class IRequestHandler {
        public:
            virtual ~IRequestHandler() {};
            virtual std::string handle(const std::string &input) = 0;
        };

        class IListener {
        public:
            virtual ~IListener() {}
            virtual bool start() = 0;
            virtual bool stop() = 0;
        };
    }

    namespace disposing {
        class IDisposable {
        public:
            virtual ~IDisposable() {}
            virtual void dispose() = 0;
        };

        class DummyDisposable : public IDisposable {
        public:
            virtual ~DummyDisposable() {}
            virtual void dispose() override {}
        };

        template<typename TEntry>
        using unique_ptr = std::unique_ptr<TEntry, std::function<void(TEntry*)>>;

        template <typename TEntry>
        class IEntryProvider {
        public:
            virtual ~IEntryProvider() {}
            virtual core::disposing::unique_ptr<TEntry> get() = 0;
        };

        template <typename TEntry>
        class NotDisposableResource : public core::disposing::IEntryProvider<TEntry> {
        private:
            TEntry *_entry;
        public:
            NotDisposableResource(TEntry *entry)
                : _entry(entry) {}
            virtual ~NotDisposableResource() {}
            virtual core::disposing::unique_ptr<TEntry> get() override {
                return core::disposing::makeDummy<TEntry>(_entry);
            }
        };

        template <typename TEntry>
        inline unique_ptr<TEntry> makeDummy(TEntry *entry) {
            return unique_ptr<TEntry>(entry, [](TEntry *entry) {});
        }

        inline unique_ptr<void> makeHandle(HANDLE entry) {
            return unique_ptr<void>(entry, [](HANDLE entry) { ::CloseHandle(entry); });
        }

        template <typename TEntry>
        inline unique_ptr<TEntry> makePointer(TEntry *entry) {
            return unique_ptr<TEntry>(entry, [](TEntry *entry) { SAFE_DELETE(entry) });
        }

        template<typename TEntry>
        inline unique_ptr<TEntry> makeUnknown(TEntry *entry) {
            return unique_ptr<TEntry>(entry, [](TEntry *entry) { SAFE_RELEASE(entry); });
        }
    }

    namespace algorithm {
        float gaussian(float x, float sigma);
        uint64_t hash64(const void *key, unsigned int length, unsigned int resolution, unsigned int seed);
    }

    namespace exception {
        class InitializationException : public std::exception {
        private:
            std::string _message;
        public:
            InitializationException(std::string type, std::string reason) {
                _message = core::stringFormat("%s initialization failed, due to %s", type.c_str(), reason.c_str());
            }
            virtual ~InitializationException() {}
            virtual const char* what() const throw() {
                return _message.c_str();
            }
        };

        class OperationException : public std::exception {
        private:
            std::string _message;
        public:
            OperationException(std::string type, std::string reason) {
                _message = core::stringFormat("%s operation failed, due to %s", type.c_str(), reason.c_str());
            }
            virtual ~OperationException() {}
            virtual const char* what() const throw() {
                return _message.c_str();
            }
        };

        class ResourceNotFoundException : public std::exception {
        private:
            std::string _message;
        public:
            ResourceNotFoundException(std::string key) {
                _message = core::stringFormat("resource '%s' not found", key.c_str());
            }
            virtual ~ResourceNotFoundException() {}
            virtual const char* what() const throw() {
                return _message.c_str();
            }
        };
    }

    namespace file {
        std::string addTrailingSlash(const std::string &path);
        std::string combine(const std::string &first, const std::string &second);
        bool fileExists(const std::string &filePath);
        std::string getDirectory(const std::string &directoryPath);
        void readToEnd(std::string filePath, std::string &fileContent);
        std::string removeLeadingSlash(const std::string &path);
    }

    namespace linq {
        template <typename TInput>
        class Enumerable {
        private:
            std::list<TInput> _input;
        public:
            Enumerable(std::list<TInput> input)
                : _input(input) {}
            int indexOf(std::function<bool(const TInput&)> predicate) {
                for (auto it = _input.begin(); it != _input.end(); ++it) {
                    if (predicate(*it)) {
                        return static_cast<int>(std::distance(_input.begin(), it));
                    }
                }

                return -1;
            }
            bool contains(std::function<bool(const TInput&)> predicate) {
                return indexOf(predicate) > -1;
            }
            template <typename TOutput>
            Enumerable<TOutput> ofType() {
                std::list<TOutput> output;
                for (auto entry : _input) {
                    if (std::is_base_of<TOutput, TInput>::value) {
                        output.push_back((TOutput)entry);
                    }
                }

                return Enumerable<TOutput>(output);
            }
            template <typename TOutput>
            Enumerable<TOutput> select(std::function<TOutput(const TInput&)> selector) {
                std::list<TOutput> output;
                for (auto entry : _input) {
                    output.push_back(selector(entry));
                }

                return Enumerable<TOutput>(output);
            }
            Enumerable<TInput> where(std::function<bool(const TInput&)> predicate) {
                std::list<TInput> output;
                for (auto entry : _input) {
                    if (predicate(entry)) {
                        output.push_back(entry);
                    }
                }

                return Enumerable<TInput>(output);
            }
            TInput firstOrDefault(TInput default) {
                if (!_input.empty()) {
                    return _input.front();
                }

                return default;
            }
            Enumerable<TInput> distinct(std::function<bool(const TInput&, const TInput&)> predicate) {
                std::list<TInput> output;
                for (auto first : _input) {
                    bool found = false;
                    for (auto second : output) {
                        if (predicate(first, second)) {
                            found = true;
                            break;
                        }
                    }

                    if (!found) {
                        output.push_back(first);
                    }
                }

                return Enumerable<TInput>(output);
            }
            std::list<TInput> toList() {
                std::list<TInput> output(_input.begin(), _input.end());
                return output;
            }
            std::vector<TInput> toVector() {
                std::vector<TInput> output(_input.begin(), _input.end());
                return output;
            }
        };

        template <typename TInput>
        Enumerable<TInput> makeEnumerable(std::list<TInput> collection) {
            return Enumerable<TInput>(collection);
        }

        template <typename TInput>
        Enumerable<TInput> makeEnumerable(std::vector<TInput> collection) {
            std::list<TInput> input(collection.begin(), collection.end());
            return Enumerable<TInput>(input);
        }

        template <typename TInput>
        Enumerable<TInput> makeEnumerable(TInput *collection, size_t size) {
            std::list<TInput> input;
            for (size_t i = 0; i < size; i++) {
                input.push_back(collection[i]);
            }

            return Enumerable<TInput>(input);
        }
    }

    namespace logging {
        enum LogLevel {
            information = 0,
            debug = 1,
            warn = 2,
            error = 3
        };

        class ILogTextAppender {
        public:
            virtual ~ILogTextAppender() {}
            virtual void append(std::string &text) = 0;
        };

        class ILogger {
        public:
            virtual ~ILogger() {}
            virtual std::unique_ptr<disposing::IDisposable> runInScope(std::unique_ptr<ILogTextAppender> appender) = 0;
            virtual void log(const std::string &text) = 0;
            virtual void log(LogLevel logLevel, const std::string &text) = 0;
        };

        class ILoggerFactory {
        public:
            virtual ~ILoggerFactory() {}
            virtual std::unique_ptr<ILogger> create(std::string source) = 0;
        };

        class FakeLogger final : public ILogger {
        public:
            ~FakeLogger() {}
            virtual std::unique_ptr<disposing::IDisposable> runInScope(std::unique_ptr<ILogTextAppender> appender) override {
                return std::unique_ptr<disposing::IDisposable>(new disposing::DummyDisposable());
            }
            virtual void log(const std::string &text) override {
            }
            virtual void log(LogLevel logLevel, const std::string &text) override {
            };
        };

        class Logger {
        public:
            static std::unique_ptr<ILogger> current;
        };

        class ThreadLogTextAppender final : public ILogTextAppender {
        private:
            std::thread::id _id;
            std::string _prefix;
        public:
            ThreadLogTextAppender(std::string prefix);
            virtual ~ThreadLogTextAppender() {}
            virtual void append(std::string &text) override;
        };

        class ThreadLoggerAppenderScope final {
        private:
            ILogger *_logger;
            LogLevel _logLevel;
            std::unique_ptr<disposing::IDisposable> _scope;
        public:
            ThreadLoggerAppenderScope(std::string prefix)
                : ThreadLoggerAppenderScope(information, prefix) {}
            ThreadLoggerAppenderScope(LogLevel logLevel, std::string prefix)
                : ThreadLoggerAppenderScope(Logger::current.get(), logLevel, prefix) {}
            ThreadLoggerAppenderScope(ILogger *logger, LogLevel logLevel, std::string prefix) {
                _logger = logger;
                _logLevel = logLevel;
                _scope = _logger->runInScope(std::unique_ptr<ILogTextAppender>(new ThreadLogTextAppender(core::stringFormat("> %s: ", prefix.c_str()))));
                _logger->log(_logLevel, "start");
            }
            ~ThreadLoggerAppenderScope() {
                _logger->log(_logLevel, "done");
                _scope->dispose();
            }
        };

        static void log(const std::string &text) {
            Logger::current->log(text);
        }

        static void log(LogLevel logLevel, const std::string &text) {
            Logger::current->log(logLevel, text);
        }

        static void log(LogLevel logLevel, std::string text, HRESULT result) {
            if (SUCCEEDED(result)) {
                Logger::current->log(logLevel, core::stringFormat("%s completed", text.c_str()));
            }
            else {
                Logger::current->log(error, core::stringFormat("%s failed with error code: 0x%08lx", text.c_str(), result));
            }
        }
    }

    class IHookTransaction {
    public:
        virtual ~IHookTransaction() {}
        virtual bool commit() = 0;
        virtual void hook(const std::string &identifier, LPVOID target, LPVOID detour, LPVOID *original) = 0;
    };

    class IHookService {
    public:
        virtual ~IHookService() {}
        virtual std::unique_ptr<IHookTransaction> openTransaction() = 0;
        virtual bool unhookAll() = 0;
    };

    namespace time {
        class DateTime {
        public:
            unsigned short year;
            unsigned short month;
            unsigned short day;
            unsigned short hour;
            unsigned short minute;
            unsigned short second;
            unsigned short milliseconds;
            long long totalMiliseconds;
        };

        std::string toString(const DateTime &time);

        class ITimeProvider {
        public:
            virtual ~ITimeProvider() {}
            virtual DateTime getTime() = 0;
        };
    }
}