#!/usr/bin/env bash
# new Env("hostloc每日访问主页")
# cron 40 9 * * * ray_hostloc_visitSpaces.sh

. ray_hostloc_base.sh

cd ./src/Ray.Quartz.Hostloc
export DOTNET_ENVIRONMENT=Production && \
export Ray_Hostloc_Run=visitSpace && \
dotnet run --ENVIRONMENT=Production