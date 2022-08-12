#!/usr/bin/env bash
# new Env("hostloc每日访问主页")
# cron 40 9 * * * ray_hostloc_visitSpaces.sh

dir_shell=$QL_DIR/shell
. $dir_shell/share.sh

hostloc_repo="raywangqvq_ray.quartz.hostloc"

echo "repo目录: $dir_repo"
hostloc_repo_dir="$(find $dir_repo -type d -name $hostloc_repo | head -1)"
echo -e "hostloc仓库目录: $hostloc_repo_dir\n"

cd $hostloc_repo_dir/src/Ray.Quartz.Hostloc
export DOTNET_ENVIRONMENT=Production && \
export Ray_Hostloc_Run=visitSpace && \
dotnet run