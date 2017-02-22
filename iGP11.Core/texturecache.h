#pragma once

#include "stdafx.h"
#include "igp11core.h"

namespace core {
	typedef std::function<void(TextureProfile &profile, bool value)> BoolMappingAction;
	typedef std::function<void(TextureProfile &profile, std::string value)> TextMappingAction;

	class IValueMapper {
	public:
		virtual ~IValueMapper() {}
		virtual bool isApplicable(const std::string &propertyRow) = 0;
		virtual void apply(TextureProfile &profile, const std::string &propertyRow) = 0;
	};

	class BoolValueMapper : public IValueMapper {
	private:
		std::string _key;
		BoolMappingAction _mappingAction;
	public:
		BoolValueMapper(std::string key, BoolMappingAction mappingAction)
			: _key(key), _mappingAction(mappingAction) {}
		virtual ~BoolValueMapper() {}
		virtual bool isApplicable(const std::string &propertyRow) override;
		virtual void apply(TextureProfile &profile, const std::string &propertyRow) override;
	};

	class TextValueMapper : public IValueMapper {
	private:
		std::string _key;
		TextMappingAction _mappingAction;
	public:
		TextValueMapper(std::string key, TextMappingAction mappingAction)
			: _key(key), _mappingAction(mappingAction) {}
		virtual ~TextValueMapper() {}
		virtual bool isApplicable(const std::string &propertyRow) override;
		virtual void apply(TextureProfile &profile, const std::string &propertyRow) override;
	};

	class StateTextureCacheVisitor : public ITextureCacheVisitor {
	private:
		std::list<std::string> _profiles;
	public:
		virtual ~StateTextureCacheVisitor() {}
		virtual void visit(const TextureProfile &profile) override;
		std::string build();
	};

	class TextureCache : public ITextureCache {
	private:
		std::list<std::shared_ptr<TextureProfile>> _profiles;
	public:
		TextureCache(std::string directoryPath);
		virtual ~TextureCache() {}
		virtual size_t getCount() override;
		virtual bool has(const std::string &id) override;
		virtual void accept(ITextureCacheVisitor &visitor) override;
		virtual std::shared_ptr<TextureProfile> find(const std::string &id) override;
		virtual void merge(TextureProfile &profile) override;
	private:
		std::list<std::string> getTextures(const std::string &directoryPath);
		std::map<std::string, std::list<std::string>> getTextureProfiles(const std::string &directoryPath);
		void cache(const std::string &directoryPath);
	};

	class TextureCacheFactory : public ITextureCacheFactory {
	public:
		virtual ~TextureCacheFactory() {}
		virtual std::shared_ptr<ITextureCache> createFromDirectory(std::string directoryPath) override;
	};
}