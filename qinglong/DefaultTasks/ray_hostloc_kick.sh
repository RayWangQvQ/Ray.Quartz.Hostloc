#!/usr/bin/env bash
# new Env("hostloc踢楼")
# cron 0 * * * * ray_hostloc_kick.sh

. ray_hostloc_base.sh

cd ./src/Ray.Quartz.Hostloc
export DOTNET_ENVIRONMENT=Production && \
export Ray_Hostloc_Run=kick && \
dotnet run --ENVIRONMENT=Production