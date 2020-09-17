set remoteDirGcc="se260454@copy18-1.hpc.itc.rwth-aachen.de:~/Mocassin/deploy_1.1.X.X/bin_gcc/"
set remoteDirIcc="se260454@copy18-1.hpc.itc.rwth-aachen.de:~/Mocassin/deploy_1.1.X.X/bin_icc/"
set localDirGcc="C:\Users\hims-user\Documents\Gitlab\Mocassin.Builds_1.1.X.X\simulator_linux"
set localDirIcc="C:\Users\hims-user\Documents\Gitlab\Mocassin.Builds_1.1.X.X\simulator_linux"
scp -r %remoteDirGcc% %localDirGcc%
scp -r %remoteDirIcc% %localDirIcc%