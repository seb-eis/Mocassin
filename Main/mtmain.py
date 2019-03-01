import subprocess as subprocess
import threading as threading

def runSimulator(args):
    executable = r"C:\Users\hims-user\source\repos\ICon.Simulator\cmake-build-release-mingw_x86_64\Mocassin.Simulator.exe"
    result = subprocess.call(args=args, executable=executable)
    return result

def runMocassin(dbQuery, dbPath, stdRedirect):
    args = [r"C:\Users\hims-user\source\repos\ICon.Simulator", "-dbQuery", str(dbQuery), "-dbPath", str(dbPath), "-stdRedirect", str(stdRedirect)]
    thread = threading.Thread(target=runSimulator, args=(args,))
    return thread


def main():
    ceriadB = r"C:\Users\hims-user\Documents\Gitlab\MocassinTestFiles\YDopedCeria\Ceria.db"
    threads = []
    for i in range(0, 1):
        redirect = r".\out_" + str(i) + ".log"
        thread = runMocassin(2, ceriadB, redirect)
        threads.append(thread)
        thread.start()

    for thread in threads:
        thread.join()

main()
