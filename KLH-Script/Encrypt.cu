

%%writefile password_cracker.cu
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

__device__ char* CudaCrypt(char* rawPassword) {
    char *newPassword = (char *) malloc(sizeof(char) * 11);

    newPassword[0] = rawPassword[0] + 2;
    newPassword[1] = rawPassword[0] - 2;
    newPassword[2] = rawPassword[0] + 1;
    newPassword[3] = rawPassword[1] + 3;
    newPassword[4] = rawPassword[1] - 3;
    newPassword[5] = rawPassword[1] - 1;
    newPassword[6] = rawPassword[2] + 2;
    newPassword[7] = rawPassword[2] - 2;
    newPassword[8] = rawPassword[3] + 4;
    newPassword[9] = rawPassword[3] - 4;
    newPassword[10] = '\0';

    for (int i = 0; i < 10; i++) {
        if (i >= 0 && i < 6) { // checking all uppercase letter limits
            if (newPassword[i] > 90) {
                newPassword[i] = (newPassword[i] - 90) + 65;
            } else if (newPassword[i] < 65) {
                newPassword[i] = (65 - newPassword[i]) + 65;
            }
        } else { // checking number section
            if (newPassword[i] > 57) {
                newPassword[i] = (newPassword[i] - 57) + 48;
            } else if (newPassword[i] < 48) {
                newPassword[i] = (48 - newPassword[i]) + 48;
            }
        }
    }
    return newPassword;
}

__global__ void crack(char *alphabet, char *numbers, char *userPassword, unsigned long long *combinationCount, unsigned long long *lastThreadId) {
    char genRawPass[4];

    // Generating password characters
    genRawPass[0] = alphabet[blockIdx.x];
    genRawPass[1] = alphabet[blockIdx.y];
    genRawPass[2] = numbers[threadIdx.x];
    genRawPass[3] = numbers[threadIdx.y];

    // Calculate unique thread ID
    unsigned long long threadId = (blockIdx.x * gridDim.y * blockDim.x * blockDim.y) +
                                  (blockIdx.y * blockDim.x * blockDim.y) +
                                  (threadIdx.x * blockDim.y) +
                                  threadIdx.y;

    // Compare the generated password with the user input
    bool match = true;
    for (int i = 0; i < 4; i++) {
        if (genRawPass[i] != userPassword[i]) {
            match = false;
            break;
        }
    }

    if (match) {
        // If the password matches, print the thread ID and the generated password
        printf("Thread ID: %llu | %c %c %c %c = %s\n", threadId, genRawPass[0], genRawPass[1], genRawPass[2], genRawPass[3], CudaCrypt(genRawPass));
    }

    // Update combination count
    atomicAdd(combinationCount, 1ULL);

    // Record the last thread ID
    atomicMax(lastThreadId, threadId);
}

int main(int argc, char **argv) {
    char cpuAlphabet[26] = {'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};
    char cpuNumbers[10] = {'0','1','2','3','4','5','6','7','8','9'};

    char *gpuAlphabet;
    cudaMalloc((void**) &gpuAlphabet, sizeof(char) * 26);
    cudaMemcpy(gpuAlphabet, cpuAlphabet, sizeof(char) * 26, cudaMemcpyHostToDevice);

    char *gpuNumbers;
    cudaMalloc((void**) &gpuNumbers, sizeof(char) * 10);
    cudaMemcpy(gpuNumbers, cpuNumbers, sizeof(char) * 10, cudaMemcpyHostToDevice);

    // Device variables for tracking total combinations and last thread ID
    unsigned long long *d_combinationCount, *d_lastThreadId;
    unsigned long long h_combinationCount = 0, h_lastThreadId = 0;
    cudaMalloc((void**) &d_combinationCount, sizeof(unsigned long long));
    cudaMalloc((void**) &d_lastThreadId, sizeof(unsigned long long));
    cudaMemcpy(d_combinationCount, &h_combinationCount, sizeof(unsigned long long), cudaMemcpyHostToDevice);
    cudaMemcpy(d_lastThreadId, &h_lastThreadId, sizeof(unsigned long long), cudaMemcpyHostToDevice);

    // Prompt user for a password to search for
    char userPassword[5];
    printf("Enter a 4-character password to search for: ");
    scanf("%4s", userPassword);

    // Allocate memory for user password on the GPU
    char *gpuUserPassword;
    cudaMalloc((void**)&gpuUserPassword, sizeof(char) * 5);
    cudaMemcpy(gpuUserPassword, userPassword, sizeof(char) * 5, cudaMemcpyHostToDevice);

    // Launch the kernel
    dim3 grid(26, 26, 1);
    dim3 block(10, 10, 1);
    crack<<<grid, block>>>(gpuAlphabet, gpuNumbers, gpuUserPassword, d_combinationCount, d_lastThreadId);
    cudaDeviceSynchronize();

    // Copy results back to host
    cudaMemcpy(&h_combinationCount, d_combinationCount, sizeof(unsigned long long), cudaMemcpyDeviceToHost);
    cudaMemcpy(&h_lastThreadId, d_lastThreadId, sizeof(unsigned long long), cudaMemcpyDeviceToHost);

    // Print summary
    printf("Total number of passwords explored: %llu\n", h_combinationCount);
    printf("Last thread ID: %llu\n", h_lastThreadId);

    // Cleanup
    cudaFree(gpuAlphabet);
    cudaFree(gpuNumbers);
    cudaFree(gpuUserPassword);
    cudaFree(d_combinationCount);
    cudaFree(d_lastThreadId);

    return 0;
}


