#!/usr/bin/env bash
# new Env("hostloc每日投票")
# cron 5 12,23 * * * ray_hostloc_vote.sh

dir_shell=$QL_DIR/shell
. $dir_shell/share.sh
. /root/.bashrc

cd ./src/Ray.Quartz.Hostloc
# export DOTNET_ENVIRONMENT=Production && \
export Ray_Hostloc_Run=reply && \
dotnet run --ENVIRONMENT=Production