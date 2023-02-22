#!/usr/bin/env bash
# new Env("hostloc每日登录")
# cron 30 9 * * * ray_hostloc_login.sh

. ray_hostloc_base.sh

cd ./src/Ray.Quartz.Hostloc
# export DOTNET_ENVIRONMENT=Production && \
export Ray_Hostloc_Run=login && \
dotnet run --ENVIRONMENT=Production