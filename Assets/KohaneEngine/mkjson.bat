@echo off
CD /D %~dp0
ykmc kohane_engine.ykm --target-json ./test.json
pause