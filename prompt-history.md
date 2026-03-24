# Prompt History

Chronological log of prompts used to build this project with Claude Code.

## Session 1 — Scaffold baseline

1. `/clean-architecture:install-clean-arch-rules` — Install architecture rules into `rules/`
2. `/clean-architecture:bootstrap-clean-arch` — Scaffold C# clean architecture projects and generate per-module CLAUDE.md files

## Session 2 — React SPA + Todo feature

1. `feat: scaffold React SPA and update clean architecture rules` — Added React 19 SPA with Vite, React Query, Zustand, clean architecture layers (domain, repositories, services, state, components, pages), and wired it into the solution
2. `open url http://localhost:5173/ and debug` — Fixed Vite proxy target (port 7001 -> 49595 to match launchSettings.json), fixed empty `avatarUrl` causing React warnings on `<img src="">`, updated README for React SPA, updated .gitignore for React/Playwright
