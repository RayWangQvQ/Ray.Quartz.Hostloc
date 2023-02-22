#!/usr/bin/env bash
# new Env("hostloc踢楼")
# cron 5 12,23 * * * ray_hostloc_reply.sh

. ray_hostloc_base.sh

cd ./src/Ray.Quartz.Hostloc
# export DOTNET_ENVIRONMENT=Production && \
export Ray_Hostloc_Run=reply && \
dotnet run --ENVIRONMENT=Production