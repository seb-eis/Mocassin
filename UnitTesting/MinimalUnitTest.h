#ifndef ICON_SIMULATOR_MINIMALUNITTEST_H
#define ICON_SIMULATOR_MINIMALUNITTEST_H

#endif //ICON_SIMULATOR_MINIMALUNITTEST_H

#define mu_assert(message, test) do { if (!(test)) return message; } while (0)
#define mu_run_test(test) do { char *message = test(); tests_run++; \
                                if (message) return message; } while (0)
extern int tests_run;
extern char error_message[265];