# Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/)
and this project adheres to [Semantic Versioning](http://semver.org/).

## [2.1.1] - 2024-08-07

### Added

- Enrich with object (also includes support of anonymous types)
- Enrich with dictionary
- Ability to add activity without creating context (`IActivityScope.Add` method)
- Default ActivitySaveChangesInterceptor
- Added ability to modified entity values equality

### Fixed

- Enrich with DateOnly, TimeOnly, DateTime, DateTimeOffset, TimeSpan, Uri

### Changed

- Delete EntityChange doesn't contain OriginalValues anymore, Values would be filled in with OriginalValues

## [2.1.0-rc1] - 2024-04-19

### Added

- EntityFrameworkCore entity change interceptors
