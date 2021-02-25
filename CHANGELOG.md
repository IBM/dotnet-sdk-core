# [1.2.0](https://github.com/IBM/dotnet-sdk-core/compare/v1.1.0...v1.2.0) (2021-02-25)


### Bug Fixes

* **build:** main migration ([796bb00](https://github.com/IBM/dotnet-sdk-core/commit/796bb00178a1685e7c25302d2ffc1e19060d2512))
* **build:** main migration automation ([6528d3e](https://github.com/IBM/dotnet-sdk-core/commit/6528d3ec9eb95783b5a076ce63bd7ae42b4dcbdd))
* **build:** main migration automation ([3e4ceea](https://github.com/IBM/dotnet-sdk-core/commit/3e4ceea2efa77c4c743393de470f84bcb3a9aae6))
* allow '=' character in environment config values ([761da72](https://github.com/IBM/dotnet-sdk-core/commit/761da72124c66f03a3ecd97cec257363e957c0c8))


### Features

* **readme:** trigger a release ([439b576](https://github.com/IBM/dotnet-sdk-core/commit/439b576a39505ae18a4a91a4ae1c3d7b057ce447))

# [1.2.0](https://github.com/IBM/dotnet-sdk-core/compare/v1.1.0...v1.2.0) (2021-02-16)


### Bug Fixes

* **build:** main migration ([796bb00](https://github.com/IBM/dotnet-sdk-core/commit/796bb00178a1685e7c25302d2ffc1e19060d2512))
* **build:** main migration automation ([6528d3e](https://github.com/IBM/dotnet-sdk-core/commit/6528d3ec9eb95783b5a076ce63bd7ae42b4dcbdd))
* **build:** main migration automation ([3e4ceea](https://github.com/IBM/dotnet-sdk-core/commit/3e4ceea2efa77c4c743393de470f84bcb3a9aae6))
* allow '=' character in environment config values ([761da72](https://github.com/IBM/dotnet-sdk-core/commit/761da72124c66f03a3ecd97cec257363e957c0c8))


### Features

* **readme:** trigger a release ([439b576](https://github.com/IBM/dotnet-sdk-core/commit/439b576a39505ae18a4a91a4ae1c3d7b057ce447))

## [1.1.1](https://github.com/IBM/dotnet-sdk-core/compare/v1.1.0...v1.1.1) (2021-02-15)


### Bug Fixes

* **build:** main migration ([796bb00](https://github.com/IBM/dotnet-sdk-core/commit/796bb00178a1685e7c25302d2ffc1e19060d2512))
* **build:** main migration automation ([6528d3e](https://github.com/IBM/dotnet-sdk-core/commit/6528d3ec9eb95783b5a076ce63bd7ae42b4dcbdd))
* **build:** main migration automation ([3e4ceea](https://github.com/IBM/dotnet-sdk-core/commit/3e4ceea2efa77c4c743393de470f84bcb3a9aae6))
* allow '=' character in environment config values ([761da72](https://github.com/IBM/dotnet-sdk-core/commit/761da72124c66f03a3ecd97cec257363e957c0c8))

## [1.1.1](https://github.com/IBM/dotnet-sdk-core/compare/v1.1.0...v1.1.1) (2021-02-11)


### Bug Fixes

* **build:** main migration ([796bb00](https://github.com/IBM/dotnet-sdk-core/commit/796bb00178a1685e7c25302d2ffc1e19060d2512))
* **build:** main migration automation ([6528d3e](https://github.com/IBM/dotnet-sdk-core/commit/6528d3ec9eb95783b5a076ce63bd7ae42b4dcbdd))
* **build:** main migration automation ([3e4ceea](https://github.com/IBM/dotnet-sdk-core/commit/3e4ceea2efa77c4c743393de470f84bcb3a9aae6))
* allow '=' character in environment config values ([761da72](https://github.com/IBM/dotnet-sdk-core/commit/761da72124c66f03a3ecd97cec257363e957c0c8))

# [1.1.0](https://github.com/IBM/dotnet-sdk-core/compare/v1.0.1...v1.1.0) (2019-12-06)


### Features

* **IBMService:** enhance vcap parsing ([5c26d73](https://github.com/IBM/dotnet-sdk-core/commit/5c26d73fde549a3f1fe18cdd938679b0120d39bc))

## [1.0.1](https://github.com/IBM/dotnet-sdk-core/compare/v1.0.0...v1.0.1) (2019-11-23)


### Bug Fixes

* **IAM Token:** Update token validation ([357fc03](https://github.com/IBM/dotnet-sdk-core/commit/357fc0368577c18cc61bdd19705dbdbb4413cb54))

# [1.0.0](https://github.com/IBM/dotnet-sdk-core/compare/v0.8.2...v1.0.0) (2019-10-03)


### Bug Fixes

* **Endpoint:** Remove trailing slash from endpoint if it exists ([40c28d1](https://github.com/IBM/dotnet-sdk-core/commit/40c28d1))


### Features

* **Authentication:** Update auth methods ([e9095e2](https://github.com/IBM/dotnet-sdk-core/commit/e9095e2))
* **AuthType:** Ignore case of authType ([5f14b25](https://github.com/IBM/dotnet-sdk-core/commit/5f14b25))
* **Config order:** Look for config file in working directory before home directory ([c8cf60b](https://github.com/IBM/dotnet-sdk-core/commit/c8cf60b))
* **DisableSslVerification:** Debug message when an http request results in an ssl error ([ed022aa](https://github.com/IBM/dotnet-sdk-core/commit/ed022aa))
* **FileWithMetadata:** Add FileWithMetadata model ([e9e892e](https://github.com/IBM/dotnet-sdk-core/commit/e9e892e))
* **SetServiceUrl:** Refactored SetEndpoint to SetServiceUrl ([feab2d9](https://github.com/IBM/dotnet-sdk-core/commit/feab2d9))


### BREAKING CHANGES

* **Config order:** SDK now looks for the config file in the working directory before looking in the
home directory
* **SetServiceUrl:** Use SetServiceUrl to set the service endpoint rather than SetEndpoint
* **Authentication:** Auth methods were updated and old authentication methods were removed

## [0.8.2](https://github.com/IBM/dotnet-sdk-core/compare/v0.8.1...v0.8.2) (2019-07-22)


### Bug Fixes

* **Query params:** Query params were alredy being escaped ([6a055c6](https://github.com/IBM/dotnet-sdk-core/commit/6a055c6))

## [0.8.1](https://github.com/IBM/dotnet-sdk-core/compare/v0.8.0...v0.8.1) (2019-07-22)


### Bug Fixes

* **Query parameters:** Escape query parameters ([adac980](https://github.com/IBM/dotnet-sdk-core/commit/adac980))

# [0.8.0](https://github.com/IBM/dotnet-sdk-core/compare/v0.7.1...v0.8.0) (2019-06-28)


### Features

* **Authentication:** Support authentication via config files, add ICP4D support ([4da7902](https://github.com/IBM/dotnet-sdk-core/commit/4da7902))

## [0.7.1](https://github.com/IBM/dotnet-sdk-core/compare/v0.7.0...v0.7.1) (2019-05-06)


### Bug Fixes

* Refactored SendAsInsecure to DisableSslVerification ([68e0e95](https://github.com/IBM/dotnet-sdk-core/commit/68e0e95))

# [0.7.0](https://github.com/IBM/dotnet-sdk-core/compare/v0.6.0...v0.7.0) (2019-05-03)


### Features

* **Custom request headers:** Added a method to add headers via Dictionary ([a71679f](https://github.com/IBM/dotnet-sdk-core/commit/a71679f))

# [0.6.0](https://github.com/IBM/dotnet-sdk-core/compare/v0.5.3...v0.6.0) (2019-05-03)


### Features

* **Custom request headers:** Add method to get headers ([b0940eb](https://github.com/IBM/dotnet-sdk-core/commit/b0940eb))
* **Custom request headers:** Add support for custom request headers ([14a7f17](https://github.com/IBM/dotnet-sdk-core/commit/14a7f17))

## [0.5.3](https://github.com/IBM/dotnet-sdk-core/compare/v0.5.2...v0.5.3) (2019-05-02)


### Bug Fixes

* **Request:** Clear default accept headers on instantiation ([24dc6ca](https://github.com/IBM/dotnet-sdk-core/commit/24dc6ca))

## [0.5.2](https://github.com/IBM/dotnet-sdk-core/compare/v0.5.1...v0.5.2) (2019-04-29)


### Bug Fixes

* **TokenManager:** Add null check for tokenResponse when saving tokenInfo ([d32c4d7](https://github.com/IBM/dotnet-sdk-core/commit/d32c4d7))

## [0.5.1](https://github.com/IBM/dotnet-sdk-core/compare/v0.5.0...v0.5.1) (2019-04-29)


### Bug Fixes

* **Request:** Added a null check for no content in the response ([91d9e4c](https://github.com/IBM/dotnet-sdk-core/commit/91d9e4c))
* **Request:** Remove string parsing of status code to long ([4ce5bbc](https://github.com/IBM/dotnet-sdk-core/commit/4ce5bbc))

# [0.5.0](https://github.com/IBM/dotnet-sdk-core/compare/v0.4.0...v0.5.0) (2019-04-26)


### Features

* **DynamicModel:** Added unrestricted DynamicModel base class ([c64650a](https://github.com/IBM/dotnet-sdk-core/commit/c64650a))

# [0.4.0](https://github.com/IBM/dotnet-sdk-core/compare/v0.3.0...v0.4.0) (2019-04-24)


### Features

* **Additional properties:** Added DynamicModel base class for models with AdditionalProperties ([d2d9386](https://github.com/IBM/dotnet-sdk-core/commit/d2d9386))

# [0.3.0](https://github.com/IBM/dotnet-sdk-core/compare/v0.2.1...v0.3.0) (2019-04-09)


### Features

* **Float culture:** Parse floats to strings with invariant culture ([9789c93](https://github.com/IBM/dotnet-sdk-core/commit/9789c93))

## [0.2.1](https://github.com/IBM/dotnet-sdk-core/compare/v0.2.0...v0.2.1) (2019-04-09)


### Bug Fixes

* Look for iam_apikey in credential file for IAM auth ([0bd16c3](https://github.com/IBM/dotnet-sdk-core/commit/0bd16c3))

# [0.2.0](https://github.com/IBM/dotnet-sdk-core/compare/v0.1.0...v0.2.0) (2019-04-09)


### Features

* **CustomData and BaseModel:** Removed CustomData and BaseModel ([2489b67](https://github.com/IBM/dotnet-sdk-core/commit/2489b67))
* **DetailedResponse:** Added DetailedResponse ([34d1d06](https://github.com/IBM/dotnet-sdk-core/commit/34d1d06))

# [0.1.0](https://github.com/IBM/dotnet-sdk-core/compare/v0.0.1...v0.1.0) (2019-03-29)


### Features

* **ErrorMessage:** Standardized errorMessage parsing ([89e4979](https://github.com/IBM/dotnet-sdk-core/commit/89e4979))

# 1.0.0 (2019-03-29)


### Features

* Refactored Watson to IBM ([7d9065f](https://github.com/IBM/dotnet-sdk-core/commit/7d9065f))
* Refactored Watson to IBM ([3f43bab](https://github.com/IBM/dotnet-sdk-core/commit/3f43bab))
