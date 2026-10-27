# Agent skills

These skills teach agents how to build Visual Studio extensions. They are copied unchanged from [`madskristensen/vs-agent-plugins`](https://github.com/madskristensen/vs-agent-plugins) at commit `e971073e2163d4bc66d3e455e102c316e01c94c6`; the upstream is licensed under the Apache License 2.0, and its [`LICENSE`](LICENSE) is next to this file.

| Skill | Use it for |
|---|---|
| `handling-async-threading` | async/await, UI-thread switches, `JoinableTaskFactory` |
| `handling-extension-errors` | exceptions, the Output window, the activity log |
| `theming-extension-ui` | WPF UI that follows the Visual Studio theme |
| `adding-options-settings` | Tools > Options pages and settings |
| `providing-consuming-services` | Visual Studio services and service providers |
| `interacting-solution-explorer` | selecting, expanding and collapsing Solution Explorer nodes from code |

Only the skills that apply to this extension are copied. The whole catalogue (40 skills) is the Claude Code plugin `vs-extensibility-skills` that `.claude/settings.json` enables, so Claude Code may list the skills above twice: once from this folder and once from the plugin.

`Directory.Build.targets` links this folder to `.claude/skills` (git-ignored) for Claude Code; GitHub Copilot in Visual Studio reads the skills from here.

To update a skill, copy its `SKILL.md` from a newer commit of the upstream repository and change the commit above.
