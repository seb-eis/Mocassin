//
// Created by john on 08.10.2018.
//

#ifndef ICON_SIMULATOR_UNITTESTS_H
#define ICON_SIMULATOR_UNITTESTS_H

#endif //ICON_SIMULATOR_UNITTESTS_H

#include "MinimalUnitTest.h"
#include "Framework/Sqlite/SqliteReader.h"
#include <stdio.h>
#include "ExternalLibraries/sqlite3.h"
#include <string.h>
#include <stdlib.h>
#include <stdint.h>

int tests_run = 0;

typedef Span_t(int32_t, IntegerSpan) IntergerSpan_t;

static char* testDatabase()
{
    mu_assert("error! could not execute sql query", TestDatabase("../Database/InteropTest.db") == SQLITE_OK);
    return 0;
};

static char* ExecuteTestQuery()
{
    mu_assert("error! could not load and execute test query", TestQuery("../Database/InteropTest.db") == 0);
    return 0;
}

static char* TestSpans()
{
    IntergerSpan_t integerSpan;
    new_Span(integerSpan, 10);

    int i = 0;
    cpp_foreach(intIter, integerSpan)
    {
        *intIter = i;
        i++;
    }

    cpp_foreach(intIter, integerSpan)
    {
        printf("current value: %d \n", *intIter);
    }

    delete_Span(integerSpan);

    return 0;
}

static char * all_tests() {
    mu_run_test(testDatabase);
    mu_run_test(ExecuteTestQuery);
    mu_run_test(TestSpans);
    return 0;
};

int startTesting(int argc, char **argv) {

    char *result = all_tests();
    if (result != 0) {
        printf("%s\n", result);
    }
    else {
        printf("ALL TESTS PASSED\n");
    }
    printf("Tests run: %d\n", tests_run);

    return result != 0;
};
